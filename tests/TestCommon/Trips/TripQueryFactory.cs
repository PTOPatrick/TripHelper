using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Trips.Queries.GetTrip;
using TripHelper.Application.Trips.Queries.GetTrips;

namespace TestCommon.Trips;

public static class TripQueryFactory
{
    public static GetTripsQuery CreateGetTripsQuery()
    {
        return new GetTripsQuery();
    }

    public static GetTripQuery CreateGetTripQuery(int tripId)
    {
        return new GetTripQuery(tripId);
    }

    public static GetTripsQueryHandler CreateGetTripsQueryHandler(
        IMembersRepository membersRepository,
        ITripsRepository tripsRepository,
        IAuthorizationService authorizationService)
    {
        return new GetTripsQueryHandler(membersRepository, tripsRepository, authorizationService);
    }

    public static GetTripQueryHandler CreateGetTripQueryHandler(
        IMembersRepository membersRepository,
        ITripsRepository tripsRepository,
        IUsersRepository usersRepository,
        IAuthorizationService authorizationService)
    {
        return new GetTripQueryHandler(tripsRepository, membersRepository, authorizationService, usersRepository);
    }
}