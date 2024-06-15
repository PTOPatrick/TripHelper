namespace TripHelper.Contracts.Users;

public record CreateUserRequest(string Email, string Password, string Firstname, string Lastname, bool IsSuperAdmin);