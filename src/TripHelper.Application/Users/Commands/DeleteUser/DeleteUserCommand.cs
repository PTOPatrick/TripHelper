using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;

namespace TripHelper.Application.Users.Commands.DeleteUser;

[Authorize]
public record DeleteUserCommand(int UserId) : IRequest<ErrorOr<Deleted>>;