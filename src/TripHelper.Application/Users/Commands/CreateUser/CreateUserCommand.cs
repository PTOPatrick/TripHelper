using ErrorOr;
using MediatR;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Firstname, string Lastname, string Password, string Email, bool IsSuperAdmin) : IRequest<ErrorOr<User>>;