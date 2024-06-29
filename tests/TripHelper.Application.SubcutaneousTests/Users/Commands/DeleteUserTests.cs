using MediatR;
using TestCommon.Users;
using TestCommon.TestConstants;
using TripHelper.Application.SubcutaneousTests.Common;
using FluentAssertions;
using TripHelper.Domain.Users;
using NSubstitute;
using TripHelper.Application.Common.Interfaces;
using ErrorOr;
using TestCommon.Members;

namespace TripHelper.Application.SubcutaneousTests.Users.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class DeleteUserTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task DeleteUser_WhenValidData_ShouldDeleteUser()
    {
        // Arrange
        var user = await CreateUser();
        _authorizationService.CanDeleteUser(user.Id).Returns(true);
        _usersRepository.GetUserByIdAsync(user.Id).Returns(user);

        var command = UserCommandFactory
            .CreateDeleteUserCommand(user.Id);

        var handler = UserCommandFactory.CreateDeleteUserCommandHandler(
            _usersRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);
    }

    [Fact]
    public async Task DeleteUser_WhenInValidData_ShouldReturnUserNotFound()
    {
        // Arrange
        _authorizationService.CanDeleteUser(Constants.User.Id).Returns(true);
        _usersRepository.GetUserByIdAsync(Constants.User.Id).Returns(null as User);

        var command = UserCommandFactory
            .CreateDeleteUserCommand(Constants.User.Id);

        var handler = UserCommandFactory.CreateDeleteUserCommandHandler(
            _usersRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task DeleteUser_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var unauthorizedError = Error.Unauthorized();

        _authorizationService.CanDeleteUser(Constants.User.Id).Returns(false);
        _usersRepository.GetUserByIdAsync(Constants.User.Id).Returns(null as User);

        var command = UserCommandFactory
            .CreateDeleteUserCommand(Constants.User.Id);

        var handler = UserCommandFactory.CreateDeleteUserCommandHandler(
            _usersRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(unauthorizedError);
    }

    private async Task<User> CreateUser()
    {
        // Arrange
        var createUserCommand = UserCommandFactory.CreateCreateUserCommand(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            false
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