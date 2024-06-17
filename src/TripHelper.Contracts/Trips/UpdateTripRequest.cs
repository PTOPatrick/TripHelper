namespace TripHelper.Contracts.Trips;

public record UpdateTripRequest(string Name, DateTime? StartDate, DateTime? EndDate, string? Description, string? Location, string? ImageUrl);