using MediatR;
using NSubstitute;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.SubcutaneousTests.Common;
using TestCommon.TestConstants;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.Members;
using TripHelper.Domain.Users;
using TestCommon.Members;
using FluentAssertions;
using TestCommon.Users;
using TripHelper.Domain.TripItems;
using TripHelper.Domain.Trips;
using TestCommon.TripItems;
using TestCommon.Trips;
using ErrorOr;

namespace TripHelper.Application.SubcutaneousTests.TripItems.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetTripItemTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly ITripItemsRepository _tripItemsRepository = Substitute.For<ITripItemsRepository>();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    
    [Fact]
    public async Task GetTripItem_WithValidData_ShouldReturnTripItemWithEmail()
    {
        // Arrange
        var trip = await CreateTrip();
        var tripItem = await CreateTripItem(trip);
        var user = await CreateUser();
        var memberWithEmail = await CreateMemberWithEmail(user);

        _authorizationService.CanGetTripItem(trip.Id).Returns(true);
        _tripItemsRepository.GetTripItemAsync(tripItem.Id).Returns(tripItem);
        _membersRepository.GetMemberAsync(tripItem.MemberId).Returns(TypeCastMemberWithEmailToMember(memberWithEmail));
        _usersRepository.GetUserByIdAsync(memberWithEmail.UserId).Returns(user);

        var query = TripItemQueryFactory.CreateGetTripItemQuery(trip.Id, tripItem.Id);
        var handler = TripItemQueryFactory.CreateGetTripItemQueryHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().Be(tripItem.Id);
        result.Value.Name.Should().Be(tripItem.Name);
        result.Value.Amount.Should().Be(tripItem.Amount);
        result.Value.MemberId.Should().Be(tripItem.MemberId);
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetTripItem_WithUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var trip = await CreateTrip();
        var tripItem = await CreateTripItem(trip);

        _authorizationService.CanGetTripItem(trip.Id).Returns(false);

        var query = TripItemQueryFactory.CreateGetTripItemQuery(trip.Id, tripItem.Id);
        var handler = TripItemQueryFactory.CreateGetTripItemQueryHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.Unauthorized());
    }

    [Fact]
    public async Task GetTripItem_WhenTripItemNotFound_ShouldReturnTripItemNotFound()
    {
        // Arrange
        var trip = await CreateTrip();
        var tripItem = await CreateTripItem(trip);

        _authorizationService.CanGetTripItem(trip.Id).Returns(true);
        _tripItemsRepository.GetTripItemAsync(tripItem.Id).Returns(null as TripItem);

        var query = TripItemQueryFactory.CreateGetTripItemQuery(trip.Id, tripItem.Id);
        var handler = TripItemQueryFactory.CreateGetTripItemQueryHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TripItemErrors.TripItemNotFound);
    }

    [Fact]
    public async Task GetTripItem_WhenMemberNotFound_ShouldReturnMemberNotFound()
    {
        // Arrange
        var trip = await CreateTrip();
        var tripItem = await CreateTripItem(trip);

        _authorizationService.CanGetTripItem(trip.Id).Returns(true);
        _tripItemsRepository.GetTripItemAsync(tripItem.Id).Returns(tripItem);
        _membersRepository.GetMemberAsync(tripItem.MemberId).Returns(null as Member);

        var query = TripItemQueryFactory.CreateGetTripItemQuery(trip.Id, tripItem.Id);
        var handler = TripItemQueryFactory.CreateGetTripItemQueryHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TripItemErrors.MemberNotFound);
    }

    [Fact]
    public async Task GetTripItem_WhenUserNotFound_ShouldReturnUserNotFound()
    {
        // Arrange
        var trip = await CreateTrip();
        var tripItem = await CreateTripItem(trip);
        var user = await CreateUser();
        var memberWithEmail = await CreateMemberWithEmail(user);

        _authorizationService.CanGetTripItem(trip.Id).Returns(true);
        _tripItemsRepository.GetTripItemAsync(tripItem.Id).Returns(tripItem);
        _membersRepository.GetMemberAsync(tripItem.MemberId).Returns(TypeCastMemberWithEmailToMember(memberWithEmail));
        _usersRepository.GetUserByIdAsync(memberWithEmail.UserId).Returns(null as User);

        var query = TripItemQueryFactory.CreateGetTripItemQuery(trip.Id, tripItem.Id);
        var handler = TripItemQueryFactory.CreateGetTripItemQueryHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TripItemErrors.UserNotFound);
    }

    private static Member TypeCastMemberWithEmailToMember(MemberWithEmail memberWithEmail)
    {
        return new Member(memberWithEmail.UserId, memberWithEmail.TripId, memberWithEmail.IsAdmin);
    }

    private async Task<MemberWithEmail> CreateMemberWithEmail(User user)
    {
        // Arrange
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

    private async Task<TripItem> CreateTripItem(Trip trip)
    {
        // Arrange
        _authorizationService.CanCreateTripItem(trip.Id).Returns(true);

        var command = TripItemCommandFactory.CreateCreateTripItemCommand(
            Constants.TripItem.Name,
            trip.Id,
            Constants.TripItem.Amount,
            Constants.TripItem.MemberId
        );

        var handler = TripItemCommandFactory.CreateCreateTripItemCommandHandler(
            _tripItemsRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Amount.Should().Be(Constants.TripItem.Amount);
        result.Value.Name.Should().Be(Constants.TripItem.Name);
        result.Value.MemberId.Should().Be(Constants.TripItem.MemberId);

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
}