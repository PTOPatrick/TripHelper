using NSubstitute;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Application.Common.Models;
using FluentAssertions;
using Constants = TestCommon.TestConstants.Constants;

namespace TripHelper.Application.UnitTests.Common.Services.Authorization;

public class AuthorizationServiceTests
{
    private readonly AuthorizationService _sut;

    public AuthorizationServiceTests()
    {
        // create a mock of the AuthorizationService
        _sut = Substitute.For<AuthorizationService>();
    }

    #region User
    [Fact]
    public void ReadUpdateDeleteUser_WhenUserIsSuperAdminOnDifferentUser_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act 
        var createUserResult = _sut.CanGetUser(Constants.User.DifferentUserId);
        var updateUserResult = _sut.CanUpdateUser(Constants.User.DifferentUserId);
        var deleteUserResult = _sut.CanDeleteUser(Constants.User.DifferentUserId);

        // Assert
        createUserResult.Should().BeTrue();
        updateUserResult.Should().BeTrue();
        deleteUserResult.Should().BeTrue();
    }

    [Fact]
    public void ReadUpdateDeleteUser_WhenUserIsSuperAdminOnSameUser_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act 
        var getUserResult = _sut.CanGetUser(Constants.User.Id);
        var updateUserResult = _sut.CanUpdateUser(Constants.User.Id);
        var deleteUserResult = _sut.CanDeleteUser(Constants.User.Id);

        // Assert
        getUserResult.Should().BeTrue();
        updateUserResult.Should().BeTrue();
        deleteUserResult.Should().BeTrue();
    }

    [Fact]
    public void CreateUser_WhenUserIsSuperAdmin_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = _sut.CanCreateUser();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CreateReadUpdateDeleteUser_WhenUserIsRegularUserOnDifferentUser_ShouldReturnFalse()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var readUserResult = _sut.CanCreateUser();
        var getUserResult = _sut.CanGetUser(Constants.User.DifferentUserId);
        var updateUserResult = _sut.CanUpdateUser(Constants.User.DifferentUserId);
        var deleteUserResult = _sut.CanDeleteUser(Constants.User.DifferentUserId);

        // Assert
        readUserResult.Should().BeFalse();
        getUserResult.Should().BeFalse();
        updateUserResult.Should().BeFalse();
        deleteUserResult.Should().BeFalse();
    }
    
    [Fact]
    public void ReadUpdateDeleteUser_WhenUserIsRegularOnSameUser_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var readUserResult = _sut.CanGetUser(Constants.User.Id);
        var updateUserResult = _sut.CanUpdateUser(Constants.User.Id);
        var deleteUserResult = _sut.CanDeleteUser(Constants.User.Id);

        // Assert
        readUserResult.Should().BeTrue();
        updateUserResult.Should().BeTrue();
        deleteUserResult.Should().BeTrue();
    }

    [Fact]
    public void CreateUser_WhenUserIsRegularUser_ShouldReturnFalse()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = _sut.CanCreateUser();

        // Assert
        result.Should().BeFalse();
    }
    #endregion

    #region Trip
    [Fact]
    public void CreateReadUpdateDeleteTrip_WhenUserIsSuperAdmin_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = 
            _sut.CanCreateTrip() &&
            _sut.CanGetTrip(Constants.User.DifferentTripId) &&
            _sut.CanUpdateTrip(Constants.User.DifferentTripId) &&
            _sut.CanDeleteTrip(Constants.User.DifferentTripId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CreateReadUpdateDeleteTrip_WhenUserIsAdmin_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = 
            _sut.CanCreateTrip() &&
            _sut.CanGetTrip(Constants.User.AdminTripIds[0]) &&
            _sut.CanUpdateTrip(Constants.User.AdminTripIds[0]) &&
            _sut.CanDeleteTrip(Constants.User.AdminTripIds[0]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CreateReadTrip_WhenUserIsRegularUser_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = 
            _sut.CanCreateTrip() &&
            _sut.CanGetTrip(Constants.User.UserTripIds[0]);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void UpdateDeleteTrip_WhenUserIsRegularUser_ShouldReturnFalse()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = 
            _sut.CanUpdateTrip(Constants.User.UserTripIds[0]) &&
            _sut.CanDeleteTrip(Constants.User.UserTripIds[0]);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ReadUpdateDeleteTrip_WhenUserIsNotMemberOfTrip_ShouldReturnFalse()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = 
            _sut.CanGetTrip(Constants.User.DifferentTripId) &&
            _sut.CanUpdateTrip(Constants.User.DifferentTripId) &&
            _sut.CanDeleteTrip(Constants.User.DifferentTripId);

        // Assert
        result.Should().BeFalse();
    }
    #endregion

    #region TripItem
    [Fact]
    public void CreateReadUpdateDelete_WhenUserIsSuperAdmin_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var createTripItemResult = _sut.CanCreateTripItem(Constants.User.DifferentTripId);
        var readTripItemResult = _sut.CanGetTripItem(Constants.User.DifferentTripId);
        var updateTripItemResult = _sut.CanUpdateTripItem(Constants.User.DifferentTripId);
        var deleteTripItemResult = _sut.CanDeleteTripItem(Constants.User.DifferentTripId);

        // Assert
        createTripItemResult.Should().BeTrue();
        readTripItemResult.Should().BeTrue();
        updateTripItemResult.Should().BeTrue();
        deleteTripItemResult.Should().BeTrue();
    }

    [Fact]
    public void CreateTripItem_WhenUserIsMemberOfTrip_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var result = _sut.CanCreateTripItem(Constants.User.UserTripIds[0]);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CreateReadUpdateDeleteTripItem_WhenUserIsRegularUserAndNotMemberOfTrip_ShouldReturnFalse()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var createTripItemResult = _sut.CanCreateTripItem(Constants.User.DifferentTripId);
        var readTripItemResult = _sut.CanGetTrip(Constants.User.DifferentTripId);
        var updateTripItemResult = _sut.CanUpdateTrip(Constants.User.DifferentTripId);
        var deleteTripItemResult = _sut.CanDeleteTrip(Constants.User.DifferentTripId);

        // Assert
        createTripItemResult.Should().BeFalse();
        readTripItemResult.Should().BeFalse();
        updateTripItemResult.Should().BeFalse();
        deleteTripItemResult.Should().BeFalse();
    }
    
    [Fact]
    public void CreateReadUpdateDeleteTripItem_WhenUserIsAdminInTripAndMemberOfTrip_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var createTripItemResult = _sut.CanCreateTripItem(Constants.User.AdminTripIds[0]);
        var readTripItemResult = _sut.CanGetTrip(Constants.User.AdminTripIds[0]);
        var updateTripItemResult = _sut.CanUpdateTrip(Constants.User.AdminTripIds[0]);
        var deleteTripItemResult = _sut.CanDeleteTrip(Constants.User.AdminTripIds[0]);

        // Assert
        createTripItemResult.Should().BeTrue();
        readTripItemResult.Should().BeTrue();
        updateTripItemResult.Should().BeTrue();
        deleteTripItemResult.Should().BeTrue();
    }
    #endregion

    #region Member
    [Fact]
    public void CreateReadUpdateDeleteMember_WhenUserIsSuperAdmin_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.SuperAdminUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var createMemberResult = _sut.CanCreateMember(Constants.User.DifferentTripId);
        var readMemberResult = _sut.CanGetMember(Constants.User.DifferentTripId);
        var updateMemberResult = _sut.CanUpdateMember(Constants.User.DifferentTripId);
        var deleteMemberResult = _sut.CanDeleteMember(Constants.User.DifferentTripId);

        // Assert
        createMemberResult.Should().BeTrue();
        readMemberResult.Should().BeTrue();
        updateMemberResult.Should().BeTrue();
        deleteMemberResult.Should().BeTrue();
    }
    
    [Fact]
    public void CreateReadUpdateDeleteMember_WhenUserIsAdminInTrip_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var createMemberResult = _sut.CanCreateMember(Constants.User.AdminTripIds[0]);
        var readMemberResult = _sut.CanGetMember(Constants.User.AdminTripIds[0]);
        var updateMemberResult = _sut.CanUpdateMember(Constants.User.AdminTripIds[0]);
        var deleteMemberResult = _sut.CanDeleteMember(Constants.User.AdminTripIds[0]);

        // Assert
        createMemberResult.Should().BeTrue();
        readMemberResult.Should().BeTrue();
        updateMemberResult.Should().BeTrue();
        deleteMemberResult.Should().BeTrue();
    }
    
    [Fact]
    public void CreateReadMember_WhenUserIsRegularUserInTrip_ShouldReturnTrue()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var createMemberResult = _sut.CanCreateMember(Constants.User.UserTripIds[0]);
        var getMemberResult = _sut.CanGetMember(Constants.User.UserTripIds[0]);

        // Assert
        createMemberResult.Should().BeTrue();
        getMemberResult.Should().BeTrue();
    }
    
    [Fact]
    public void CreateUpdateDeleteMember_WhenUserIsRegularUserInTrip_ShouldReturnFalse()
    {
        // Arrange
        var currentUser = new CurrentUser(
            Constants.User.Id,
            Constants.User.Permissions, 
            Constants.User.RegularUserRoles, 
            Constants.User.UserTripIds, 
            Constants.User.AdminTripIds
        );
        _sut.InjectLogic(currentUser);

        // Act
        var updateMemberResult = _sut.CanUpdateMember(Constants.User.UserTripIds[0]);
        var deleteMemberResult = _sut.CanDeleteMember(Constants.User.UserTripIds[0]);

        // Assert
        updateMemberResult.Should().BeFalse();
        deleteMemberResult.Should().BeFalse();
    }
    #endregion
}