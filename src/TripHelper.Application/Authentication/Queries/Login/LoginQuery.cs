using ErrorOr;
using MediatR;
using TripHelper.Application.Authentication.Common;

namespace TripHelper.Application.Authentication.Queries.Login;

public record LoginQuery(string Email, string Password) : IRequest<ErrorOr<AuthenticationResult>>;