using TripHelper.Domain.Trips;

namespace TestCommon.Trips;

public static class TripFactory
{
    public static Trip CreateTrip(string name, DateTime? startDate, DateTime? endDate, string? description, string? location, string? imageUrl, int creatorUserId)
    {
        return new Trip(name, startDate, endDate, description, location, imageUrl, creatorUserId);
    }
}