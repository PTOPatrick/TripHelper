using ErrorOr;
using MediatR;
using TripHelper.Application.Authentication.Commands.Register;
using TripHelper.Application.Authentication.Common;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Common.Interfaces;
using TripHelper.Domain.Users;

namespace GymManagement.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IJwtTokenGenerator _jwtTokenGenerator,
    IPasswordHasher _passwordHasher,
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork)
        : IRequestHandler<RegisterCommand, ErrorOr<AuthenticationResult>>
{
    public async Task<ErrorOr<AuthenticationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await _usersRepository.ExistsByEmailAsync(command.Email))
            return Error.Conflict(description: "User already exists");

        var hashPasswordResult = _passwordHasher.HashPassword(command.Password);

        if (hashPasswordResult.IsError)
            return hashPasswordResult.Errors;

        var user = new User(command.Email, command.Firstname, command.Lastname, hashPasswordResult.Value, false);

        await _usersRepository.AddUserAsync(user);
        await _unitOfWork.CommitChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user, []);

        return new AuthenticationResult(user, token);
    }
}