using MediatR;
using TestCommon.Members;
using TripHelper.Application.SubcutaneousTests.Common;
using TestCommon.TestConstants;
using TripHelper.Domain.Members;
using FluentAssertions;
using TripHelper.Application.Common.Interfaces;
using NSubstitute;
using ErrorOr;
using TripHelper.Domain.Users;
using TripHelper.Domain.Trips;
using TestCommon.Users;
using TestCommon.Trips;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.SubcutaneousTests.Members.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class UpdateMemberTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IMembersRepository _membersRepository = Substitute.For<IMembersRepository>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task UpdateMember_WithMemberDoesNotExist_ShouldReturnMemberNotFound()
    {
        // Arrange
        var command = MemberCommandFactory.CreateUpdateMemberCommand(Constants.Member.Id, false);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(MemberErrors.MemberNotFound);
    }

    [Fact]
    public async Task UpdateMember_WithWhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();
        var member = CreateMockMember(user, trip);

        _authorizationService.CanUpdateMember(Constants.Trip.Id).Returns(false);
        _membersRepository.GetMemberAsync(member.Id).Returns(member);

        var command = MemberCommandFactory.CreateUpdateMemberCommand(member.Id, false);
        var handler = MemberCommandFactory.CreateUpdateMemberCommandHandler(_membersRepository, _usersRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.Unauthorized());
    }

    [Fact]
    public async Task UpdateMember_WithWhenInvalidUser_ShouldReturnUserNotFound()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();
        var member = CreateMockMember(user, trip);

        _authorizationService.CanUpdateMember(Constants.Trip.Id).Returns(true);
        _membersRepository.GetMemberAsync(member.Id).Returns(member);

        var command = MemberCommandFactory.CreateUpdateMemberCommand(member.Id, false);
        var handler = MemberCommandFactory.CreateUpdateMemberCommandHandler(_membersRepository, _usersRepository, _unitOfWork, _authorizationService);
        _usersRepository.GetUserByIdAsync(member.Id).Returns(null as User);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(MemberErrors.UserNotFound);
    }

    [Fact]
    public async Task UpdateMember_WithWhenValidData_ShouldReturnMemberWithEmail()
    {
        // Arrange
        var user = await CreateUser();
        var trip = await CreateTrip();
        var member = CreateMockMember(user, trip);
        var memberWithEmail = GetMemberWithEmail(user, member);

        _usersRepository.GetUserByIdAsync(user.Id).Returns(user);
        _authorizationService.CanUpdateMember(Constants.Trip.Id).Returns(true);
        _membersRepository.GetMemberAsync(member.Id).Returns(member);

        var command = MemberCommandFactory.CreateUpdateMemberCommand(member.Id, false);
        var handler = MemberCommandFactory.CreateUpdateMemberCommandHandler(_membersRepository, _usersRepository, _unitOfWork, _authorizationService);
        _usersRepository.GetUserByIdAsync(member.Id).Returns(user);

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