using MediatR;
using NSubstitute;
using TestCommon.Members;
using TestCommon.Trips;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.SubcutaneousTests.Common;
using TripHelper.Domain.Trips;
using TestCommon.TestConstants;
using FluentAssertions;
using ErrorOr;
using TripHelper.Domain.Users;
using TestCommon.Users;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.Members;

namespace TripHelper.Application.SubcutaneousTests.Members.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetTripMembersTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task GetTripMembers_WithUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var trip = await CreateTrip();

        _authorizationService.CanGetTrip(trip.Id).Returns(false);

        var command = MemberQueryFactory.CreateGetTripMembersQuery(trip.Id);
        var handler = MemberQueryFactory.CreateGetTripMembersQueryHandler(_membersRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.Unauthorized());
    }

    [Fact]
    public async Task GetTripMembers_WithAuthorized_ShouldReturnTripMembers()
    {
        // Arrange
        var trip = await CreateTrip();
        var user = await CreateUser();
        var memberWithEmail = await CreateMemberWithEmail(user, trip);

        _authorizationService.CanGetTrip(trip.Id).Returns(false);
        _membersRepository.GetMembersByTripIdAsync(trip.Id).Returns([TypeCastMemberWithEmailToMember(memberWithEmail)]);
        _usersRepository.GetUsersByIdsAsync([user.Id]).Returns([user]);

        var command = MemberQueryFactory.CreateGetTripMembersQuery(trip.Id);
        var handler = MemberQueryFactory.CreateGetTripMembersQueryHandler(_membersRepository, _usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.Unauthorized());
    }

    private static Member TypeCastMemberWithEmailToMember(MemberWithEmail memberWithEmail)
    {
        return new Member(memberWithEmail.UserId, memberWithEmail.TripId, memberWithEmail.IsAdmin);
    }


    private async Task<MemberWithEmail> CreateMemberWithEmail(User user, Trip trip)
    {
        // Arrange
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