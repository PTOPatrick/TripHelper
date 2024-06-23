using MediatR;
using TripHelper.Application.SubcutaneousTests.Common;
using TestCommon.Users;
using TestCommon.TestConstants;
using FluentAssertions;


namespace TripHelper.Application.SubcutaneousTests.Users.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateUserTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    
    [Fact]
    public async Task CreateUser_WhenValidData_ShouldCreateUser()
    {
        // Arrange
        var command = UserCommandFactory.CreateCreateUserCommand(
            Constants.User.Firstname,
            Constants.User.Lastname,
            Constants.User.Password,
            Constants.User.Email,
            false
        );

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(null, "lastname", "thiIS!!@1918Aa", "example.mail@test.com")]
    [InlineData("firstname", null, "thiIS!!@1918Aa", "example.mail@test.com")]
    [InlineData("firstname", "lastname", null, "example.mail@test.com")]
    [InlineData("firstname", "lastname", "short", "example.mail@test.com")]
    [InlineData("firstname", "lastname", "nocapitalletters!@12", "example.mail@test.com")]
    [InlineData("firstname", "lastname", "nonumbersAC!!@", "example.mail@test.com")]
    [InlineData("firstname", "lastname", "NOLOWERCASELETTERS$!**", "example.mail@test.com")]
    [InlineData("firstname", "lastname", "thiIS!!@1918Aa", "")]
    [InlineData("firstname", "lastname", "thiIS!!@1918Aa", "test.com")]
    [InlineData("firstname", "lastname", "thiIS!!@1918Aa", "@test.com")]
    [InlineData("firstname", "lastname", "thiIS!!@1918Aa", "example.mail@")]
    public async Task CreateUser_WhenInValidData_ShouldReturnValidationError(string firstname, string lastname, string password, string email)
    {
        // Arrange
        var command = UserCommandFactory.CreateCreateUserCommand(
            firstname,
            lastname,
            password,
            email,
            false
        );

        // Act
        var result = await _mediator.Send(command);

        // Assert
        result.IsError.Should().BeTrue();
    }
}