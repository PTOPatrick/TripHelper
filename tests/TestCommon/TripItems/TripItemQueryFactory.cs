using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.TripItems.Queries.GetTripItem;
using TripHelper.Application.TripItems.Queries.GetTripItems;

namespace TestCommon.TripItems;

public static class TripItemQueryFactory
{
    public static GetTripItemsQuery CreateGetTripItemsQuery(int tripId)
    {
        return new GetTripItemsQuery(tripId);
    }

    public static GetTripItemQuery CreateGetTripItemQuery(int tripId, int tripItemId)
    {
        return new GetTripItemQuery(tripId, tripItemId);
    }

    public static GetTripItemsQueryHandler CreateGetTripItemsQueryHandler(
        ITripItemsRepository tripItemsRepository,
        IMembersRepository membersRepository,
        IUsersRepository usersRepository,
        IAuthorizationService authorizationService)
    {
        return new GetTripItemsQueryHandler(tripItemsRepository, membersRepository, usersRepository, authorizationService);
    }

    public static GetTripItemQueryHandler CreateGetTripItemQueryHandler(
        ITripItemsRepository tripItemsRepository,
        IMembersRepository membersRepository,
        IUsersRepository usersRepository,
        IAuthorizationService authorizationService)
    {
        return new GetTripItemQueryHandler(tripItemsRepository, membersRepository, usersRepository, authorizationService);
    }
}