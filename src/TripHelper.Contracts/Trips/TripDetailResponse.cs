namespace TripHelper.Contracts.Trips;

public record TripDetailResponse(int Id, string Name, DateTime? StartDate, DateTime? EndDate, string? Description, string? Location, string? ImageUrl, int CreatorUserId, List<string> Emails);