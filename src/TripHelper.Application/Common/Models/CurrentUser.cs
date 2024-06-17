namespace TripHelper.Application.Common.Models;

public record CurrentUser(int Id, IReadOnlyList<string> Permissions, IReadOnlyList<string> Roles, IReadOnlyList<int> UserTripIds, IReadOnlyList<int> AdminTripIds);