using System.Security.Claims;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Application.Common.Services.Authorization;

namespace TripHelper.Api.Services;

public class CurrentUserProvider(
    IHttpContextAccessor _httpContextAccessor,
    AuthorizationService _authorizationService) : ICurrentUserProvider
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
        var userTripIds = ConvertClaimValuesToList(GetClaimValues("userMember")[0]);
        var adminTripIds = ConvertClaimValuesToList(GetClaimValues("adminMember")[0]);

        var currentUser = new CurrentUser(
            Id: idClaim, 
            Permissions: permissionsClaim,
            Roles: rolesClaim,
            UserTripIds: userTripIds,
            AdminTripIds: adminTripIds);

        _authorizationService.InjectLogic(currentUser);

        return currentUser;
    }

    private IReadOnlyList<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor!.HttpContext!.User.Claims
            .Where(c => c.Type == claimType)
            .Select(c => c.Value)
            .ToList();
    }

    private List<int> ConvertClaimValuesToList(string claimValue)
    {
        var splitted = claimValue.Split(',');
        if (splitted.Length == 1 && string.IsNullOrEmpty(splitted[0]))
            return [];

        return splitted.Select(int.Parse).ToList();
    }
}