using NSubstitute;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Application.Common.Models;
using FluentAssertions;
using Constants = TestCommon.TestConstants.Constants;

namespace TripHelper.Application.UnitTests.Common.Services.Authorization;

public class AuthorizationServiceTests
{
    private readonly AuthorizationService _mockAuthorizationService;

    public AuthorizationServiceTests()
    {
        // create a mock of the AuthorizationService
        _mockAuthorizationService = Substitute.For<AuthorizationService>();
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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act 
        var createUserResult = _mockAuthorizationService.CanGetUser(Constants.User.DifferentUserId);
        var updateUserResult = _mockAuthorizationService.CanUpdateUser(Constants.User.DifferentUserId);
        var deleteUserResult = _mockAuthorizationService.CanDeleteUser(Constants.User.DifferentUserId);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act 
        var getUserResult = _mockAuthorizationService.CanGetUser(Constants.User.Id);
        var updateUserResult = _mockAuthorizationService.CanUpdateUser(Constants.User.Id);
        var deleteUserResult = _mockAuthorizationService.CanDeleteUser(Constants.User.Id);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = _mockAuthorizationService.CanCreateUser();

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var readUserResult = _mockAuthorizationService.CanCreateUser();
        var getUserResult = _mockAuthorizationService.CanGetUser(Constants.User.DifferentUserId);
        var updateUserResult = _mockAuthorizationService.CanUpdateUser(Constants.User.DifferentUserId);
        var deleteUserResult = _mockAuthorizationService.CanDeleteUser(Constants.User.DifferentUserId);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var readUserResult = _mockAuthorizationService.CanGetUser(Constants.User.Id);
        var updateUserResult = _mockAuthorizationService.CanUpdateUser(Constants.User.Id);
        var deleteUserResult = _mockAuthorizationService.CanDeleteUser(Constants.User.Id);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = _mockAuthorizationService.CanCreateUser();

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = 
            _mockAuthorizationService.CanCreateTrip() &&
            _mockAuthorizationService.CanGetTrip(Constants.User.DifferentTripId) &&
            _mockAuthorizationService.CanUpdateTrip(Constants.User.DifferentTripId) &&
            _mockAuthorizationService.CanDeleteTrip(Constants.User.DifferentTripId);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = 
            _mockAuthorizationService.CanCreateTrip() &&
            _mockAuthorizationService.CanGetTrip(Constants.User.AdminTripIds[0]) &&
            _mockAuthorizationService.CanUpdateTrip(Constants.User.AdminTripIds[0]) &&
            _mockAuthorizationService.CanDeleteTrip(Constants.User.AdminTripIds[0]);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = 
            _mockAuthorizationService.CanCreateTrip() &&
            _mockAuthorizationService.CanGetTrip(Constants.User.UserTripIds[0]);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = 
            _mockAuthorizationService.CanUpdateTrip(Constants.User.UserTripIds[0]) &&
            _mockAuthorizationService.CanDeleteTrip(Constants.User.UserTripIds[0]);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = 
            _mockAuthorizationService.CanGetTrip(Constants.User.DifferentTripId) &&
            _mockAuthorizationService.CanUpdateTrip(Constants.User.DifferentTripId) &&
            _mockAuthorizationService.CanDeleteTrip(Constants.User.DifferentTripId);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var createTripItemResult = _mockAuthorizationService.CanCreateTripItem(Constants.User.DifferentTripId);
        var readTripItemResult = _mockAuthorizationService.CanGetTripItem(Constants.User.DifferentTripId);
        var updateTripItemResult = _mockAuthorizationService.CanUpdateTripItem(Constants.User.DifferentTripId);
        var deleteTripItemResult = _mockAuthorizationService.CanDeleteTripItem(Constants.User.DifferentTripId);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var result = _mockAuthorizationService.CanCreateTripItem(Constants.User.UserTripIds[0]);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var createTripItemResult = _mockAuthorizationService.CanCreateTripItem(Constants.User.DifferentTripId);
        var readTripItemResult = _mockAuthorizationService.CanGetTrip(Constants.User.DifferentTripId);
        var updateTripItemResult = _mockAuthorizationService.CanUpdateTrip(Constants.User.DifferentTripId);
        var deleteTripItemResult = _mockAuthorizationService.CanDeleteTrip(Constants.User.DifferentTripId);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var createTripItemResult = _mockAuthorizationService.CanCreateTripItem(Constants.User.AdminTripIds[0]);
        var readTripItemResult = _mockAuthorizationService.CanGetTrip(Constants.User.AdminTripIds[0]);
        var updateTripItemResult = _mockAuthorizationService.CanUpdateTrip(Constants.User.AdminTripIds[0]);
        var deleteTripItemResult = _mockAuthorizationService.CanDeleteTrip(Constants.User.AdminTripIds[0]);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var createMemberResult = _mockAuthorizationService.CanCreateMember(Constants.User.DifferentTripId);
        var readMemberResult = _mockAuthorizationService.CanGetMember(Constants.User.DifferentTripId);
        var updateMemberResult = _mockAuthorizationService.CanUpdateMember(Constants.User.DifferentTripId);
        var deleteMemberResult = _mockAuthorizationService.CanDeleteMember(Constants.User.DifferentTripId);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var createMemberResult = _mockAuthorizationService.CanCreateMember(Constants.User.AdminTripIds[0]);
        var readMemberResult = _mockAuthorizationService.CanGetMember(Constants.User.AdminTripIds[0]);
        var updateMemberResult = _mockAuthorizationService.CanUpdateMember(Constants.User.AdminTripIds[0]);
        var deleteMemberResult = _mockAuthorizationService.CanDeleteMember(Constants.User.AdminTripIds[0]);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var createMemberResult = _mockAuthorizationService.CanCreateMember(Constants.User.UserTripIds[0]);
        var getMemberResult = _mockAuthorizationService.CanGetMember(Constants.User.UserTripIds[0]);

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
        _mockAuthorizationService.InjectLogic(currentUser);

        // Act
        var updateMemberResult = _mockAuthorizationService.CanUpdateMember(Constants.User.UserTripIds[0]);
        var deleteMemberResult = _mockAuthorizationService.CanDeleteMember(Constants.User.UserTripIds[0]);

        // Assert
        updateMemberResult.Should().BeFalse();
        deleteMemberResult.Should().BeFalse();
    }
    #endregion
}