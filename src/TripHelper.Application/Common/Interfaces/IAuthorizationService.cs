using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Common.Interfaces;

public interface IAuthorizationService
{
    void InjectLogic(CurrentUser currentUser);
    int GetCurrentUserId();
    bool IsSuperAdmin();
    bool CanCreateUser();
    bool CanGetUser(int userId);
    bool CanUpdateUser(int userId);
    bool CanDeleteUser(int userId);
    bool CanCreateTrip();
    bool CanGetTrip(int tripId);
    bool CanUpdateTrip(int tripId);
    bool CanDeleteTrip(int tripId);
    bool CanCreateTripItem(int tripId);
    bool CanGetTripItem(int tripId);
    bool CanUpdateTripItem(int tripId);
    bool CanDeleteTripItem(int tripId);
    bool CanCreateMember(int tripId);
    bool CanGetMember(int tripId);
    bool CanUpdateMember(int tripId);
    bool CanDeleteMember(int tripId);
}