using ErrorOr;
using FluentValidation;
using NSubstitute;
using TestCommon.Users;
using TestCommon.TestConstants;
using TripHelper.Application.Common.Behaviors;
using TripHelper.Application.Users.Commands.CreateUser;
using TripHelper.Domain.Users;
using MediatR;
using FluentValidation.Results;
using FluentAssertions;

namespace app.Application.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var mockValidator = Substitute.For<IValidator<CreateUserCommand>>();
        var validationBehavior = new ValidationBehavior<CreateUserCommand, ErrorOr<User>>();
        var createUserRequest = UserCommandFactory.CreateCreateUserCommand(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            false
        );
        var user = UserFactory.CreateUser(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            false
        );
        var mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<User>>>();
        mockNextBehavior().Returns(user);
        mockValidator.ValidateAsync(createUserRequest, Arg.Any<CancellationToken>()).Returns(new ValidationResult());

        // Act
        var result = await validationBehavior.Handle(createUserRequest, mockNextBehavior, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(user);
    }
}