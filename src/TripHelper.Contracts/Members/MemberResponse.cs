namespace TripHelper.Contracts.Members;

public record MemberResponse(int Id, int UserId, int TripId, bool IsAdmin);