using FluentAssertions;
using TestCommon.TestConstants;
using TestCommon.Users;

namespace TripHelper.Domain.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void HasReachedMaxMembers_WhenMoreThanRegularUserAllows_ShouldFail()
    {
        // Arrange
        var user = UserFactory.CreateUser(
            Constants.User.Email, 
            Constants.User.Firstname, 
            Constants.User.Lastname, 
            Constants.User.Password, 
            false);

        // Act
        var result = user.HasReachedMaxMembers(Constants.User.MoreThanUserMaxTripCount);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasReachedMaxMembers_WhenLessThanRegularUserAllows_ShouldPass()
    {
        // Arrange
        var user = UserFactory.CreateUser(
            Constants.User.Email, 
            Constants.User.Firstname, 
            Constants.User.Lastname, 
            Constants.User.Password, 
            false);

        // Act
        var result = user.HasReachedMaxMembers(Constants.User.LesshanUserMaxTripCount);

        // Assert
        result.Should().BeFalse();
    }
    [Fact]
    public void HasReachedMaxMembers_WhenMoreThanSuperAdminAllows_ShouldFail()
    {
        // Arrange
        var user = UserFactory.CreateUser(
            Constants.User.Email, 
            Constants.User.Firstname, 
            Constants.User.Lastname, 
            Constants.User.Password, 
            true);

        // Act
        var result = user.HasReachedMaxMembers(Constants.User.MoreThanAdminMaxTripCount);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasReachedMaxMembers_WhenLessThanSuperAdminAllows_ShouldPass()
    {
        // Arrange
        var user = UserFactory.CreateUser(
            Constants.User.Email, 
            Constants.User.Firstname, 
            Constants.User.Lastname, 
            Constants.User.Password, 
            true);

        // Act
        var result = user.HasReachedMaxMembers(Constants.User.LessThanAdminMaxTripCount);

        // Assert
        result.Should().BeFalse();
    }
}