using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;
using TripHelper.Domain.Users;
using TripHelper.Infrastructure.Authentication.Claims;

namespace TripHelper.Infrastructure.Authentication.TokenGenerator;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(User user, List<Member> members)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Name, user.Firstname),
            new(JwtRegisteredClaimNames.FamilyName, user.Lastname),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("id", user.Id.ToString()),
            new("roles", GetSuperAdminRole(user))
        };

        var userRoles = members.Where(m => !m.IsAdmin).Select(m => m.TripId).ToList();
        var adminRoles = members.Where(m => m.IsAdmin).Select(m => m.TripId).ToList();

        AddReaderWriterIds(userRoles, claims);
        AddAdminIds(adminRoles, claims);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GetSuperAdminRole(User user) => user.IsSuperAdmin ? "Super Admin" : "";


    private static void AddReaderWriterIds(List<int> memberTrips, List<Claim> claims)
    {
        claims
            .AddIfValueNotNull("userMember", string.Join(",", memberTrips));
    }

    private static void AddAdminIds(List<int> memberTrips, List<Claim> claims)
    {
        claims
            .AddIfValueNotNull("adminMember", string.Join(",", memberTrips));
    }
}