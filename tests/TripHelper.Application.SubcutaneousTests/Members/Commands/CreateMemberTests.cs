using MediatR;
using TripHelper.Application.SubcutaneousTests.Common;
using NSubstitute;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Users;
using TestCommon.Users;
using FluentAssertions;
using TestCommon.TestConstants;
using TestCommon.Members;
using TripHelper.Domain.Trips;
using TestCommon.Trips;
using TripHelper.Domain.Members;


namespace TripHelper.Application.SubcutaneousTests.Members.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateMemberTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task CreateMember_WithValidData_ShouldCreateMember()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();

        _authorizationService.CanCreateMember(trip.Id).Returns(true);
        _usersRepository.GetUserByEmailAsync(user.Email).Returns(user);
        _membersRepository.GetMemberCountByUserIdAsync(user.Id).Returns(0);
        _tripsRepository.GetTripByIdAsync(trip.Id).Returns(trip);

        var command = MemberCommandFactory.CreateCreateMemberCommand(user.Email, trip.Id, false);
        var handler = MemberCommandFactory.CreateCreateMemberCommandHandler(_membersRepository, _usersRepository, _tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.UserId.Should().Be(user.Id);
        result.Value.TripId.Should().Be(trip.Id);
        result.Value.IsAdmin.Should().BeFalse();
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task CreateMember_WithInvalidUser_ShouldReturnUserNotFound()
    {
        // Arrange
        _authorizationService.CanCreateMember(Constants.Trip.Id).Returns(true);
        _usersRepository.GetUserByEmailAsync(Constants.User.Email).Returns(null as User);
        _membersRepository.GetMemberCountByUserIdAsync(Constants.User.Id).Returns(0);
        _tripsRepository.GetTripByIdAsync(Constants.Trip.Id).Returns(null as Trip);

        var command = MemberCommandFactory.CreateCreateMemberCommand(Constants.User.Email, Constants.Trip.Id, false);
        var handler = MemberCommandFactory.CreateCreateMemberCommandHandler(_membersRepository, _usersRepository, _tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.UserNotFound);
    }

    [Theory]
    [InlineData(false, 11)]
    [InlineData(true, 101)]
    public async Task CreateMember_WithInvalidMemberCount_ShouldReturnUserReachedMaxMembers(bool isSuperAdmin, int memberCount)
    {
        // Arrange
        var user = await CreateUser(isSuperAdmin);

        _authorizationService.CanCreateMember(Constants.Trip.Id).Returns(true);
        _usersRepository.GetUserByEmailAsync(user.Email).Returns(user);
        _membersRepository.GetMemberCountByUserIdAsync(user.Id).Returns(memberCount);
        _tripsRepository.GetTripByIdAsync(Constants.Trip.Id).Returns(null as Trip);

        var command = MemberCommandFactory.CreateCreateMemberCommand(user.Email, Constants.Trip.Id, false);
        var handler = MemberCommandFactory.CreateCreateMemberCommandHandler(_membersRepository, _usersRepository, _tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.UserReachedMaxMembers);
    }

    [Fact]
    public async Task CreateMember_WithInvalidTrip_ShouldReturnTripNotFound()
    {
        // Arrange
        var user = await CreateUser();

        _authorizationService.CanCreateMember(Constants.Trip.Id).Returns(true);
        _usersRepository.GetUserByEmailAsync(user.Email).Returns(user);
        _membersRepository.GetMemberCountByUserIdAsync(user.Id).Returns(1);
        _tripsRepository.GetTripByIdAsync(Constants.Trip.Id).Returns(null as Trip);

        var command = MemberCommandFactory.CreateCreateMemberCommand(user.Email, Constants.Trip.Id, false);
        var handler = MemberCommandFactory.CreateCreateMemberCommandHandler(_membersRepository, _usersRepository, _tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.TripNotFound);
    }

    [Fact]
    public async Task CreateMember_WithUserIsAlreadyMember_ShouldReturnMemberAlreadyExists()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();

        _authorizationService.CanCreateMember(Constants.Trip.Id).Returns(true);
        _usersRepository.GetUserByEmailAsync(user.Email).Returns(user);
        _membersRepository.GetMemberCountByUserIdAsync(user.Id).Returns(1);
        _tripsRepository.GetTripByIdAsync(Constants.Trip.Id).Returns(trip);
        _membersRepository.GetMemberByUserIdAndTripIdAsync(user.Id, trip.Id).Returns(CreateMockMember(user, trip));

        var command = MemberCommandFactory.CreateCreateMemberCommand(user.Email, Constants.Trip.Id, false);
        var handler = MemberCommandFactory.CreateCreateMemberCommandHandler(_membersRepository, _usersRepository, _tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberAlreadyExists);
    }

    private static Member CreateMockMember(User user, Trip trip)
    {
        return new Member(user.Id, trip.Id, false);
    }

    private async Task<Trip> CreateTrip()
    {
        var createTripCommand = TripCommandFactory.CreateCreateTripCommand(
            Constants.Trip.Name,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            Constants.Trip.Description,
            Constants.Trip.Location,
            Constants.Trip.ImageUrl
        );

        var result = await _mediator.Send(createTripCommand);

        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(Constants.Trip.Name);
        result.Value.Description.Should().Be(Constants.Trip.Description);
        result.Value.Location.Should().Be(Constants.Trip.Location);
        result.Value.ImageUrl.Should().Be(Constants.Trip.ImageUrl);

        return result.Value;
    }

    private async Task<User> CreateUser(bool isSuperAdmin = false)
    {
        // Arrange
        var createUserCommand = UserCommandFactory.CreateCreateUserCommand(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            isSuperAdmin
        );

        // Act
        var result = await _mediator.Send(createUserCommand);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Firstname.Should().Be(Constants.User.Firstname);
        result.Value.Lastname.Should().Be(Constants.User.Lastname);
        result.Value.Email.Should().Be(Constants.User.Email);

        return result.Value;
    }
}