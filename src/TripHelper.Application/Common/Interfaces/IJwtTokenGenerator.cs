using TripHelper.Domain.Members;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, List<Member> members);
}