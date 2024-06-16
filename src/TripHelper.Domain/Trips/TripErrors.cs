using ErrorOr;

namespace TripHelper.Domain.Trips;

public static class TripErrors
{
    public static readonly Error CannotCreateTripWithEmptyName = Error.Validation(
        code: "Trip.CanNotCreateTripWithEmptyName",
        description: "Cannot create trip with empty name.");

    public static readonly Error UserIsAlreadyMemberOfTrip = Error.Validation(
        code: "Trip.UserIsAlreadyMemberOfTrip",
        description: "User is already a member of the trip.");

    public static readonly Error UserIsNotMemberOfTrip = Error.Validation(
        code: "Trip.UserIsNotMemberOfTrip",
        description: "User is not a member of the trip.");

    public static readonly Error MemberNotFound = Error.Validation(
        code: "Trip.MemberNotFound",
        description: "Member not found.");

    public static readonly Error TripNotFound = Error.NotFound(
        code: "Trip.TripNotFound",
        description: "Trip not found.");

    public static readonly Error UserHasNoTrips = Error.Validation(
        code: "Trip.UserHasNoTrips",
        description: "User has no trips.");

    public static readonly Error TripsNotFound = Error.NotFound(
        code: "Trip.TripsNotFound",
        description: "Trips not found.");
}