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

namespace TripHelper.Application.SubcutaneousTests.Trips.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetTripsTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task GetTrips_WhenUserIsSuperAdmin_ShouldReturnAllTrips()
    {
        // Arrange
        var firstTrip = await CreateTrip();
        var secondTrip = await CreateTrip();
        var thridTrip = await CreateTrip();
        var fourthTrip = await CreateTrip();
        List<Trip> allTrips = [firstTrip, secondTrip, thridTrip, fourthTrip];
        var user = await CreateUser();
        List<Member> members = [
            CreateMockMember(user, firstTrip),
            CreateMockMember(user, secondTrip),
            CreateMockMember(user, thridTrip),
            CreateMockMember(user, fourthTrip)];

        _tripsRepository.GetTripsAsync().Returns(allTrips);
        _authorizationService.IsSuperAdmin().Returns(true);

        var query = TripQueryFactory.CreateGetTripsQuery();
        var handler = TripQueryFactory.CreateGetTripsQueryHandler(_membersRepository, _tripsRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetTrips_WhenUserIsRegularUser_ShouldReturnUsersTrips()
    {
        // Arrange
        var trip = await CreateTrip();
        var user = await CreateUser();
        List<Member> members = [CreateMockMember(user, trip)];

        _membersRepository.GetMembersByUserIdAsync(members[0].Id).Returns(members);
        _tripsRepository.GetTripsByIdsAsync(Arg.Any<List<int>>()).Returns([trip]);
        _authorizationService.IsSuperAdmin().Returns(false);

        var query = TripQueryFactory.CreateGetTripsQuery();
        var handler = TripQueryFactory.CreateGetTripsQueryHandler(_membersRepository, _tripsRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetTrips_WhenUserIsNoMemberOfAnyTrip_ShouldReturnUserHasNoTrips()
    {
        // Arrange
        var trip = await CreateTrip();

        _membersRepository.GetMembersByUserIdAsync(Arg.Any<int>()).Returns([]);
        _tripsRepository.GetTripsByIdsAsync(Arg.Any<List<int>>()).Returns([trip]);
        _authorizationService.IsSuperAdmin().Returns(false);

        var query = TripQueryFactory.CreateGetTripsQuery();
        var handler = TripQueryFactory.CreateGetTripsQueryHandler(_membersRepository, _tripsRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TripErrors.UserHasNoTrips);
    }

    [Fact]
    public async Task GetTrips_WhenNoTripsFound_ShouldReturnNoTripsFound()
    {
        // Arrange
        var trip = await CreateTrip();
        var user = await CreateUser();
        List<Member> members = [CreateMockMember(user, trip)];

        _membersRepository.GetMembersByUserIdAsync(members[0].Id).Returns(members);
        _tripsRepository.GetTripsByIdsAsync(Arg.Any<List<int>>()).Returns([]);
        _authorizationService.IsSuperAdmin().Returns(false);

        var query = TripQueryFactory.CreateGetTripsQuery();
        var handler = TripQueryFactory.CreateGetTripsQueryHandler(_membersRepository, _tripsRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TripErrors.TripsNotFound);
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