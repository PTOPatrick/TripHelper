using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Common.Services.Authorization;

public class AuthorizationService
{
    private bool _isSuperAdmin;
    private int _currentUserId;
    private IReadOnlyList<int> _adminTripIds = [];
    private IReadOnlyList<int> _userTripIds = [];

    public void InjectLogic(CurrentUser currentUser)
    {
        _isSuperAdmin = currentUser.Roles.Contains("Super Admin");
        _currentUserId = currentUser.Id;
        _adminTripIds = currentUser.AdminTripIds;
        _userTripIds = currentUser.UserTripIds;
    }

    public int GetCurrentUserId()
    {
        return _currentUserId;
    }

    public bool IsSuperAdmin()
    {
        return _isSuperAdmin;
    }

    public bool CanCreateUser()
    {
        return _isSuperAdmin;
    }

    public bool CanGetUser(int userId)
    {
        return _isSuperAdmin || _currentUserId == userId;
    }

    public bool CanUpdateUser(int userId)
    {
        return _isSuperAdmin || _currentUserId == userId;
    }

    public bool CanDeleteUser(int userId)
    {
        return _isSuperAdmin || _currentUserId == userId;
    }

    public bool CanCreateTrip()
    {
        return true;
    }

    public bool CanGetTrip(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId) || _userTripIds.Contains(tripId);
    }

    public bool CanUpdateTrip(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId);
    }

    public bool CanDeleteTrip(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId);
    }

    public bool CanCreateTripItem(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId) || _userTripIds.Contains(tripId);
    }

    public bool CanGetTripItem(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId) || _userTripIds.Contains(tripId);
    }

    public bool CanUpdateTripItem(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId) || _userTripIds.Contains(tripId);
    }

    public bool CanDeleteTripItem(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId) || _adminTripIds.Contains(tripId);
    }

    public bool CanCreateMember(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId) || _userTripIds.Contains(tripId);
    }

    public bool CanGetMember(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId);
    }

    public bool CanUpdateMember(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId);
    }

    public bool CanDeleteMember(int tripId)
    {
        return _isSuperAdmin || _adminTripIds.Contains(tripId);
    }
}