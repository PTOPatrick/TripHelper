using TestCommon.TestConstants;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.SubcutaneousTests.Common;

public class TestCurrentUserProvider : ICurrentUserProvider
{
    private CurrentUser? _currentUser;

    public void Returns(CurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public CurrentUser GetCurrentUser() {
        return _currentUser ?? new CurrentUser(Constants.User.DifferentUserId, Constants.User.Permissions, Constants.User.SuperAdminUserRoles, Constants.User.UserTripIds, Constants.User.AdminTripIds);
    }
}