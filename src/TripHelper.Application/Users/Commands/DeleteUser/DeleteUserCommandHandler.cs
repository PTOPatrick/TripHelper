using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Deleted>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByIdAsync(request.UserId);

        if (user is null)
            return UserErrors.UserNotFound;
            
        user.DeleteUser();

        await _usersRepository.DeleteUserAsync(user);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted; 
    }
}