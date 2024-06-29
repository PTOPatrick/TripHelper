using TripHelper.Domain.TripItems;

namespace TestCommon.TripItems;

public static class TripItemsFactory
{
    public static TripItem CreateTripItem(string name, int tripId, int memberId)
    {
        return new TripItem(name, tripId, memberId);
    }
}