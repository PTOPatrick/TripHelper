namespace TripHelper.Contracts.Trips;

public record TripItemResponse(int Id, string Name, decimal Amount, int MemberId, string Email);