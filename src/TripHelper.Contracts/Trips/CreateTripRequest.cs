namespace TripHelper.Contracts.Trips;

public record CreateTripRequest(string Name, DateTime? StartDate, DateTime? EndDate, string? Description, string? Location, string? ImageUrl);