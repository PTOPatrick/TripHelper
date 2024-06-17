namespace TripHelper.Application.Common.Models;

public record TripItemWithEmail(int Id, string Name, decimal Amount, int MemberId, string Email);