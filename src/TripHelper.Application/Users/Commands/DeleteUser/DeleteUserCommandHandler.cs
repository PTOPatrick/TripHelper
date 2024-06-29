using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IAuthorizationService _authorizationService
) : IRequestHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanDeleteUser(request.UserId))
            return Error.Unauthorized();

        var user = await _usersRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
            return UserErrors.UserNotFound;

        user.DeleteUser();

        await _usersRepository.DeleteUserAsync(user);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}