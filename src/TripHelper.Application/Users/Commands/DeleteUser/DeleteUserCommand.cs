using ErrorOr;
using MediatR;

namespace TripHelper.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(int UserId) : IRequest<ErrorOr<Deleted>>;