using System.Security.Claims;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;

namespace TripHelper.Api.Services;

public class CurrentUserProvider(IHttpContextAccessor _httpContextAccessor) : ICurrentUserProvider
{

    public CurrentUser GetCurrentUser()
    {
        if (_httpContextAccessor.HttpContext is null)
            throw new Exception("HttpContext is null");

        var idClaim = GetClaimValues("id")
            .Select(int.Parse)
            .FirstOrDefault();
        var permissionsClaim = GetClaimValues("permissions");
        var rolesClaim = GetClaimValues(ClaimTypes.Role);

        return new CurrentUser(
            Id: idClaim, 
            Permissions: permissionsClaim,
            Roles: rolesClaim);
    }

    private IReadOnlyList<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor!.HttpContext!.User.Claims
            .Where(c => c.Type == claimType)
            .Select(c => c.Value)
            .ToList();
    }
}