using MediatR;
using NSubstitute;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.SubcutaneousTests.Common;
using TestCommon.TestConstants;
using TripHelper.Domain.Trips;
using TestCommon.Trips;
using FluentAssertions;
using ErrorOr;
using TestCommon.TripItems;
using TripHelper.Domain.TripItems;
using TripHelper.Domain.Users;
using TestCommon.Users;
using TestCommon.Members;
using TripHelper.Domain.Members;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.SubcutaneousTests.TripItems.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetTripItemsTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly ITripItemsRepository _tripItemsRepository = Substitute.For<ITripItemsRepository>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task GetTripItems_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var trip = await CreateTrip();

        _authorizationService.CanGetTripItem(trip.Id).Returns(false);

        var query = TripItemQueryFactory.CreateGetTripItemsQuery(trip.Id);
        var handler = TripItemQueryFactory.CreateGetTripItemsQueryHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Error.Unauthorized());
    }

    [Fact]
    public async Task GetTripItems_WhenAuthorized_ShouldReturnTripItems()
    {
        // Arrange
        var trip = await CreateTrip();
        var tripItem = await CreateTripItem(trip);
        var user = await CreateUser();
        var memberWithEmail = await CreateMemberWithEmail(user);
        var member = TypeCastMemberWithEmailToMember(memberWithEmail);

        _authorizationService.CanGetTripItem(trip.Id).Returns(true);
        _tripItemsRepository.GetTripItemsByTripIdAsync(trip.Id).Returns([tripItem]);
        _membersRepository.GetMembersByTripIdAsync(trip.Id).Returns([member]);
        _usersRepository.GetUsersByIdsAsync(Arg.Any<List<int>>()).Returns([user]);

        var query = TripItemQueryFactory.CreateGetTripItemsQuery(trip.Id);
        var handler = TripItemQueryFactory.CreateGetTripItemsQueryHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().HaveCount(1);
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