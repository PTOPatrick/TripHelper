using MediatR;
using Microsoft.AspNetCore.Mvc;
using TripHelper.Application.Authentication.Commands.Register;
using TripHelper.Application.Authentication.Common;
using TripHelper.Application.Authentication.Queries.Login;
using TripHelper.Contracts.Authentication;

namespace TripHelper.Api.Controllers;

public class LoginController(
    ISender _sender
) : ApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var authResult = await _sender.Send(query);

        if (authResult.IsError && authResult.FirstError == AuthenticationErrors.InvalidCredentials)
        {
            return Problem(
                detail: authResult.FirstError.Description,
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return authResult.Match(
            authResult => Ok(MapToAuthResponse(authResult)),
            Problem);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = new RegisterCommand(request.Firstname, request.Lastname, request.Email, request.Password);

        var authResult = await _sender.Send(command);

        return authResult.Match(
            authResult => base.Ok(MapToAuthResponse(authResult)),
            Problem);
    }

    private static AuthenticationResponse MapToAuthResponse(AuthenticationResult authResult)
    {
        return new AuthenticationResponse(
            authResult.User.Id,
            authResult.User.Firstname,
            authResult.User.Lastname,
            authResult.User.Email,
            authResult.Token);
    }
}