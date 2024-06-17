namespace TripHelper.Contracts.Trips;

public record AddTripItemRequest(string Name, decimal Amount, int MemberId);