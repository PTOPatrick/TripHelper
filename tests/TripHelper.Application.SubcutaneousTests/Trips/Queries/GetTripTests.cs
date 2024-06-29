using FluentAssertions;
using MediatR;
using NSubstitute;
using TestCommon.Trips;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.SubcutaneousTests.Common;
using TripHelper.Domain.Trips;
using TestCommon.TestConstants;
using TripHelper.Domain.Members;
using TripHelper.Domain.Users;
using TestCommon.Users;
using ErrorOr;

namespace TripHelper.Application.SubcutaneousTests.Trips.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetTripTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task GetTrip_WhenUserIsSuperAdmin_ShouldReturnTrip()
    {
        // Arrange
        var trip = await CreateTrip();
        var user = await CreateUser();
        var member = CreateMockMember(user, trip);

        _tripsRepository.GetTripByIdAsync(trip.Id).Returns(trip);
        _membersRepository.GetMembersByTripIdAsync(trip.Id).Returns([member]);
        _usersRepository.GetUsersByIdsAsync(Arg.Any<List<int>>()).Returns([user]);
        _authorizationService.CanGetTrip(trip.Id).Returns(true);

        var query = TripQueryFactory.CreateGetTripQuery(trip.Id);
        var handler = TripQueryFactory.CreateGetTripQueryHandler(_membersRepository, _tripsRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(trip.Id);
    }

    [Fact]
    public async Task GetTrip_WhenUserIsNotSuperAdmin_ShouldReturnUnauthorized()
    {
        // Arrange
        var trip = await CreateTrip();
        var user = await CreateUser();
        var member = CreateMockMember(user, trip);

        _tripsRepository.GetTripByIdAsync(trip.Id).Returns(trip);
        _membersRepository.GetMembersByTripIdAsync(trip.Id).Returns([member]);
        _usersRepository.GetUsersByIdsAsync(Arg.Any<List<int>>()).Returns([user]);
        _authorizationService.CanGetTrip(trip.Id).Returns(false);

        var query = TripQueryFactory.CreateGetTripQuery(trip.Id);
        var handler = TripQueryFactory.CreateGetTripQueryHandler(_membersRepository, _tripsRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Error.Unauthorized());
    }

    [Fact]
    public async Task GetTrip_WhenTripNotFound_ShouldReturnTripNotFound()
    {
        // Arrange
        var trip = await CreateTrip();
        var user = await CreateUser();
        var member = CreateMockMember(user, trip);

        _tripsRepository.GetTripByIdAsync(trip.Id).Returns(null as Trip);
        _membersRepository.GetMembersByTripIdAsync(trip.Id).Returns([member]);
        _usersRepository.GetUsersByIdsAsync(Arg.Any<List<int>>()).Returns([user]);
        _authorizationService.CanGetTrip(trip.Id).Returns(true);

        var query = TripQueryFactory.CreateGetTripQuery(trip.Id);
        var handler = TripQueryFactory.CreateGetTripQueryHandler(_membersRepository, _tripsRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TripErrors.TripNotFound);
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