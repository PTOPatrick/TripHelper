using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateUserCommand, ErrorOr<User>>
{
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    
    public async Task<ErrorOr<User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var requestValidationResult = await ValidateRequest(command);
        return requestValidationResult.IsError
            ? requestValidationResult.Errors
            : await CreateUserFromRequest(command);
    }

    private async Task<User> CreateUserFromRequest(CreateUserCommand command)
    {
        var user = new User(command.Email, command.Firstname, command.Lastname, command.Password, command.IsSuperAdmin);

        await _usersRepository.AddUserAsync(user);
        await _unitOfWork.CommitChangesAsync();

        return user;
    }

    private async Task<ErrorOr<Success>> ValidateRequest(CreateUserCommand command)
    {
        var user = await _usersRepository.GetUserByEmailAsync(command.Email);
        if (user is not null)
            return UserErrors.UserAlreadyExists;
        
        return Result.Success;
    }
}