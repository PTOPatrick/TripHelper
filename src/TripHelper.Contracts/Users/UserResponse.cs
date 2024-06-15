namespace TripHelper.Contracts.Users;

public record UserResponse(int Id, string Email, string Firstname, string Lastname, bool IsSuperAdmin);