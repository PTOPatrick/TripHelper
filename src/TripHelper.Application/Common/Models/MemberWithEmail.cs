namespace TripHelper.Application.Common.Models;

public record MemberWithEmail(int Id, int UserId, int TripId, bool IsAdmin, string Email);