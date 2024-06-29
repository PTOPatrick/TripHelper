using ErrorOr;
using FluentAssertions;
using MediatR;
using NSubstitute;
using TestCommon.Members;
using TestCommon.TestConstants;
using TestCommon.Users;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.SubcutaneousTests.Common;
using TripHelper.Domain.Users;

namespace TripHelper.Application.SubcutaneousTests.Users.Queries;

[Collection(MediatorFactoryCollection.CollectionName)]
public class GetUserTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly IUsersRepository _usersRepository = Substitute.For<IUsersRepository>();

    [Fact]
    public async Task GetUser_WhenWithInvalidId_ShouldReturnNoUserFound()
    {
        // Arrange
        _authorizationService.CanGetUser(Constants.User.Id).Returns(true);

        var query = UserQueryFactory.CreateGetUserQuery(Constants.User.Id);
        var handler = UserQueryFactory.CreateGetUserQueryHandler(_usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task GetUser_WhenWithValidId_ShouldReturnUser()
    {
        // Arrange
        var user = await CreateUser();
        _authorizationService.CanGetUser(user.Id).Returns(true);
        _usersRepository.GetUserByIdAsync(user.Id).Returns(user);

        var query = UserQueryFactory.CreateGetUserQuery(user.Id);
        var handler = UserQueryFactory.CreateGetUserQueryHandler(_usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetUser_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var unauthorizedError = Error.Unauthorized();
        _authorizationService.CanGetUser(Constants.User.Id).Returns(false);
        _usersRepository.GetUserByIdAsync(Constants.User.Id).Returns(null as User);

        var query = UserQueryFactory.CreateGetUserQuery(Constants.User.Id);
        var handler = UserQueryFactory.CreateGetUserQueryHandler(_usersRepository, _authorizationService);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

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