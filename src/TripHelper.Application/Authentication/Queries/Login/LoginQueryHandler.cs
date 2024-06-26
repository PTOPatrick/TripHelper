using ErrorOr;
using MediatR;
using TripHelper.Application.Authentication.Common;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Common.Interfaces;

namespace TripHelper.Application.Authentication.Queries.Login;

public class LoginQueryHandler(
    IJwtTokenGenerator _jwtTokenGenerator,
    IMembersRepository _membersRepository,
    IPasswordHasher _passwordHasher,
    IUsersRepository _usersRepository)
        : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByEmailAsync(query.Email);
        if (user is null)
            return AuthenticationErrors.InvalidCredentials;

        var members = await _membersRepository.GetMembersByUserIdAsync(user.Id);
        
        return !user.IsCorrectPasswordHash(query.Password, _passwordHasher)
            ? AuthenticationErrors.InvalidCredentials
            : new AuthenticationResult(user!, _jwtTokenGenerator.GenerateToken(user!, members));
    }
}