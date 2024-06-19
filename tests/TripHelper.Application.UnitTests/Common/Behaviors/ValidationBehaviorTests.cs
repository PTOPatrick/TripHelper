using ErrorOr;
using FluentValidation;
using NSubstitute;
using TestCommon.Users;
using TestCommon.TestConstants;
using TripHelper.Application.Common.Behaviors;
using TripHelper.Application.Users.Commands.CreateUser;
using TripHelper.Domain.Users;
using MediatR;
using FluentAssertions;
using FluentValidation.Results;

namespace TripHelper.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateUserCommand, ErrorOr<User>> _validationBehavior;
    private readonly IValidator<CreateUserCommand> _mockValidator;
    private readonly RequestHandlerDelegate<ErrorOr<User>> _mockNextBehavior;

    public ValidationBehaviorTests()
    {
        // create a next behiavor mock
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<ErrorOr<User>>>();

        // create a validator mock
        _mockValidator = Substitute.For<IValidator<CreateUserCommand>>();

        // create a validation behavior instance
        _validationBehavior = new ValidationBehavior<CreateUserCommand, ErrorOr<User>>(_mockValidator);
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
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

        _mockNextBehavior().Returns(user);
        _mockValidator.ValidateAsync(createUserRequest, Arg.Any<CancellationToken>()).Returns(new ValidationResult());

        // Act
        var result = await _validationBehavior.Handle(createUserRequest, _mockNextBehavior, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task InvokeBehavior_WhenValidatorResultIsNotValid_ShouldReturnListOfErrors()
    {
        // Arrange
        var createUserRequest = UserCommandFactory.CreateCreateUserCommand(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            false
        );

        List<ValidationFailure> validationFailures = [new(propertyName: "foo", errorMessage: "bad foo")];
        _mockValidator.ValidateAsync(createUserRequest, Arg.Any<CancellationToken>()).Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _validationBehavior.Handle(createUserRequest, _mockNextBehavior, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Code.Should().Be("foo");
        result.FirstError.Description.Should().Be("bad foo");
    }
}