using ErrorOr;
using MediatR;
using TripHelper.Application.Authentication.Common;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Common.Interfaces;

namespace TripHelper.Application.Authentication.Queries.Login;

public class LoginQueryHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IMembersRepository membersRepository,
    IPasswordHasher passwordHasher,
    IUsersRepository usersRepository)
        : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IMembersRepository _membersRepository = membersRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUsersRepository _usersRepository = usersRepository;

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