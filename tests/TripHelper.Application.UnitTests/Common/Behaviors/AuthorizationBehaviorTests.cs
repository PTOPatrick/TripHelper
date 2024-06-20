using ErrorOr;
using FluentValidation;
using MediatR;
using NSubstitute;
using TestCommon.Users;
using TripHelper.Application.Common.Behaviors;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Users.Commands.CreateUser;
using TripHelper.Domain.Users;
using TestCommon.TestConstants;
using TripHelper.Application.Common.Models;
using FluentAssertions;

namespace TripHelper.Application.UnitTests.Common.Behaviors;

public class AuthorizationBehaviorTests
{
    private readonly AuthorizationBehavior<CreateUserCommand, ErrorOr<User>> _validationBehavior;
    private readonly RequestHandlerDelegate<ErrorOr<User>> _mockNextBehavior;
    private readonly ICurrentUserProvider _mockCurrentUserProvider;
    
    public AuthorizationBehaviorTests()
    {
        // create a next behiavor mock
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<User>>>();

        // create a current user provider mock
        _mockCurrentUserProvider = Substitute.For<ICurrentUserProvider>();

        // create a validation behavior instance
        _validationBehavior = new AuthorizationBehavior<CreateUserCommand, ErrorOr<User>>(_mockCurrentUserProvider);
    }

    [Fact]
    public async Task Handle_UserIsAuthorized_ShouldInvokeNextBehavior()
    {
        // Arrange
        var createUserRequest = UserCommandFactory.CreateCreateUserCommand(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            false
        );
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds);

        _mockCurrentUserProvider.GetCurrentUser().Returns(currentUser);

        // Act
        var result = await _validationBehavior.Handle(createUserRequest, _mockNextBehavior, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_UserIsUnauthorized_ShouldFail()
    {
        // Arrange
        var createUserRequest = UserCommandFactory.CreateCreateUserCommand(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            false
        );
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds);

        _mockCurrentUserProvider.GetCurrentUser().Returns(currentUser);

        // Act
        var result = await _validationBehavior.Handle(createUserRequest, _mockNextBehavior, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("General.Unauthorized");
        result.FirstError.Description.Should().Be("User is forbidden from taking this action");
    }
}