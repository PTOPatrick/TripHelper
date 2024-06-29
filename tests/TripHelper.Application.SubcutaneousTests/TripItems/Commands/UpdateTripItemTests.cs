using MediatR;
using TestCommon.Trips;
using TripHelper.Application.SubcutaneousTests.Common;
using TripHelper.Domain.Trips;
using TestCommon.TestConstants;
using FluentAssertions;
using TripHelper.Domain.TripItems;
using TripHelper.Application.Common.Interfaces;
using NSubstitute;
using TestCommon.TripItems;
using ErrorOr;
using FluentAssertions.Equivalency;
using TripHelper.Domain.Users;
using TestCommon.Members;
using TripHelper.Domain.Members;
using TripHelper.Application.Common.Models;
using TestCommon.Users;

namespace TripHelper.Application.SubcutaneousTests.TripItems.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class UpdateTripItemTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly ITripItemsRepository _tripItemsRepository = Substitute.For<ITripItemsRepository>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task UpdateTripItem_WhenUserIsAllowedToUpdateTripItem_ShouldReturnTripItemWithEmail()
    {
        // Arrange
        var tripItem = await CreateTripItem();
        var user = await CreateUser();
        var trip = await CreateTrip();
        var member = await CreateMember(user, trip);

        _authorizationService.CanUpdateTripItem(tripItem.TripId).Returns(true);
        _tripItemsRepository.GetTripItemAsync(tripItem.Id).Returns(tripItem);
        _membersRepository.GetMemberAsync(Constants.TripItem.UpdatedMemberId).Returns(TypeCastMemberWithEmailToMember(member));
        _usersRepository.GetUserByIdAsync(member.UserId).Returns(user);

        var command = TripItemCommandFactory.CreateUpdateTripItemCommand(
            tripItem.TripId,
            tripItem.Id,
            Constants.TripItem.UpdatedName,
            Constants.TripItem.UpdatedAmount,
            Constants.TripItem.UpdatedMemberId
        );

        var handler = TripItemCommandFactory.CreateUpdateTripItemCommandHandler(
            _tripItemsRepository,
            _membersRepository,
            _usersRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Amount.Should().Be(Constants.TripItem.UpdatedAmount);
        result.Value.Name.Should().Be(Constants.TripItem.UpdatedName);
        result.Value.MemberId.Should().Be(Constants.TripItem.UpdatedMemberId);
    }

    private async Task<TripItem> CreateTripItem()
    {
        // Arrange
        var trip = await CreateTrip();

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
        // Arrange
        var createTripCommand = TripCommandFactory.CreateCreateTripCommand(
            Constants.Trip.Name,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            Constants.Trip.Description,
            Constants.Trip.Location,
            Constants.Trip.ImageUrl
        );

        // Act
        var result = await _mediator.Send(createTripCommand);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(Constants.Trip.Name);
        result.Value.Description.Should().Be(Constants.Trip.Description);
        result.Value.Location.Should().Be(Constants.Trip.Location);
        result.Value.ImageUrl.Should().Be(Constants.Trip.ImageUrl);

        return result.Value;
    }

    private Member TypeCastMemberWithEmailToMember(MemberWithEmail memberWithEmail)
    {
        return new Member(memberWithEmail.UserId, memberWithEmail.TripId, memberWithEmail.IsAdmin);
    }

    private async Task<MemberWithEmail> CreateMember(User user, Trip trip)
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
}