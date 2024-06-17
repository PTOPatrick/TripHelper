using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Common.Interfaces;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IPasswordHasher _passwordHasher) : IRequestHandler<CreateUserCommand, ErrorOr<User>>
{
    public async Task<ErrorOr<User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var requestValidationResult = await ValidateRequest(command);

        return requestValidationResult.IsError
            ? requestValidationResult.Errors
            : await CreateUserFromRequest(command);
    }

    private async Task<ErrorOr<User>> CreateUserFromRequest(CreateUserCommand command)
    {
        var hashPasswordResult = _passwordHasher.HashPassword(command.Password);
        if (hashPasswordResult.IsError)
            return hashPasswordResult.Errors;

        var user = new User(command.Email, command.Firstname, command.Lastname, hashPasswordResult.Value, command.IsSuperAdmin);

        await _usersRepository.AddUserAsync(user);
        await _unitOfWork.CommitChangesAsync();

        return user;
    }

    private async Task<ErrorOr<Success>> ValidateRequest(CreateUserCommand command)
    {
        if (await _usersRepository.ExistsByEmailAsync(command.Email))
            return UserErrors.UserAlreadyExists;
        
        return Result.Success;
    }
}