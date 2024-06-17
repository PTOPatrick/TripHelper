using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Users.Queries.GetUser;

[Authorize]
public record GetUserQuery(int Id) : IRequest<ErrorOr<User>>;