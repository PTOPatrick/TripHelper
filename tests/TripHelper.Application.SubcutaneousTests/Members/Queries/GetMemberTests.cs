using MediatR;
using NSubstitute;
using TestCommon.Members;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.SubcutaneousTests.Common;
using TestCommon.TestConstants;
using FluentAssertions;
using TripHelper.Domain.Members;
using ErrorOr;
using TripHelper.Domain.Users;
using TestCommon.Users;
using TestCommon.Trips;
using TripHelper.Domain.Trips;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.SubcutaneousTests.Members.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetMemberTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task GetMember_WithProperPermissions_ShouldReturnMember()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();
        var member = CreateDummyMember(user, trip);
        var memberWithEmail = GetMemberWithEmail(user, member);

        _usersRepository.GetUserByIdAsync(user.Id).Returns(user);
        _membersRepository.GetMemberAsync(member.Id).Returns(member);
        _authorizationService.CanGetMember(trip.Id).Returns(true);

        var command = MemberQueryFactory.CreateGetMemberQuery(member.Id);
        var handler = MemberQueryFactory.CreateGetMemberQueryHandler(_membersRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(memberWithEmail);
    }

    private static MemberWithEmail GetMemberWithEmail(User user, Member member)
    {
        return new MemberWithEmail(member.Id, member.UserId, member.TripId, member.IsAdmin, user.Email);
    }

    [Fact]
    public async Task GetMember_WithUserDoesNotExist_ShouldReturnUserNotFound()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();
        var member = CreateDummyMember(user, trip);

        _membersRepository.GetMemberAsync(member.Id).Returns(member);
        _authorizationService.CanGetMember(trip.Id).Returns(true);
        _usersRepository.GetUserByIdAsync(user.Id).Returns(null as User);

        var command = MemberQueryFactory.CreateGetMemberQuery(member.Id);
        var handler = MemberQueryFactory.CreateGetMemberQueryHandler(_membersRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.UserNotFound);
    }

    [Fact]
    public async Task GetMember_WithMemberDoesNotExist_ShouldReturnMemberNotFound()
    {
        // Arrange
        _membersRepository.GetMemberAsync(Constants.Member.Id).Returns(null as Member);

        var command = MemberQueryFactory.CreateGetMemberQuery(Constants.Member.Id);
        var handler = MemberQueryFactory.CreateGetMemberQueryHandler(_membersRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberNotFound);
    }

    [Fact]
    public async Task GetMember_WithWhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();
        var member = CreateDummyMember(user, trip);

        _authorizationService.CanGetMember(Constants.Trip.Id).Returns(false);
        _membersRepository.GetMemberAsync(Constants.Member.Id).Returns(member);

        var command = MemberQueryFactory.CreateGetMemberQuery(Constants.Member.Id);
        var handler = MemberQueryFactory.CreateGetMemberQueryHandler(_membersRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.Unauthorized());
    }

    private Member CreateDummyMember(User user, Trip trip)
    {
        return new Member(user.Id, trip.Id, false);
    }

    private async Task<MemberWithEmail> CreateMember(User user, Trip trip)
    {
        var createMemberCommand = MemberCommandFactory.CreateCreateMemberCommand(
            user.Email,
            trip.Id,
            false
        );

        var createMemberCommandHandler = MemberCommandFactory.CreateCreateMemberCommandHandler(
            _membersRepository,
            _usersRepository,
            _tripsRepository,
            _unitOfWork,
            _authorizationService
        );

        var result = await createMemberCommandHandler.Handle(createMemberCommand, CancellationToken.None);

        return result.Value;
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