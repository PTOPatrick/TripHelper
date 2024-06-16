namespace TripHelper.Contracts.Trips;

public record UpdateTripRequest(int TripId, string Name, DateTime? StartDate, DateTime? EndDate, string? Description, string? Location, string? ImageUrl);