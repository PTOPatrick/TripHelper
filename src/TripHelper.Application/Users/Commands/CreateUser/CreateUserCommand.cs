using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Commands.CreateUser;

[Authorize(Roles = "Super Admin")]
public record CreateUserCommand(string Firstname, string Lastname, string Password, string Email, bool IsSuperAdmin) : IRequest<ErrorOr<User>>;