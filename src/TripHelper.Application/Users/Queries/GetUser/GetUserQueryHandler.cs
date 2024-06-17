using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler(
        IUsersRepository _usersRepository,
        AuthorizationService _authorizationService) : IRequestHandler<GetUserQuery, ErrorOr<User>>
    {
        public async Task<ErrorOr<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            if (!_authorizationService.CanGetUser(request.Id))
                return Error.Unauthorized();

            var user = await _usersRepository.GetUserByIdAsync(request.Id);
            if (user is null)
                return UserErrors.UserNotFound;

            return user;
        }
    }
}