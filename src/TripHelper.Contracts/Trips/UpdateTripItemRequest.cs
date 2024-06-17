namespace TripHelper.Contracts.Trips;

public record UpdateTripItemRequest(string Name, decimal Amount, int MemberId);