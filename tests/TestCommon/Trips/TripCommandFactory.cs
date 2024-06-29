using TripHelper.Application.Trips.Commands.CreateTrip;

namespace TestCommon.Trips;

public static class TripCommandFactory
{
    public static CreateTripCommand CreateCreateTripCommand(
        string name, 
        DateTime? startDate, 
        DateTime? endDate, 
        string? description, 
        string? location, 
        string? imageUrl)
    {
        return new CreateTripCommand(name, startDate, endDate, description, location, imageUrl);
    }
}