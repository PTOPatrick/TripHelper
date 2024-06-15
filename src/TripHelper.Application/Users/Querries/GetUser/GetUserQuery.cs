using ErrorOr;
using MediatR;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Queries.GetUser;

public record GetUserQuery(int Id) : IRequest<ErrorOr<User>>;