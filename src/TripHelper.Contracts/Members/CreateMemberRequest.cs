namespace TripHelper.Contracts.Members;

public record CreateMemberRequest(string Email, int TripId, bool IsAdmin = false);