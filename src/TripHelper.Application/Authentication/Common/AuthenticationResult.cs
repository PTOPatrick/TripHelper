using TripHelper.Domain.Users;

namespace TripHelper.Application.Authentication.Common;

public record AuthenticationResult(User User, string Token);