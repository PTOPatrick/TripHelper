using ErrorOr;
using MediatR;
using TripHelper.Application.Authentication.Common;

namespace TripHelper.Application.Authentication.Commands.Register;

public record RegisterCommand(string Firstname, string Lastname, string Email, string Password) : IRequest<ErrorOr<AuthenticationResult>>;