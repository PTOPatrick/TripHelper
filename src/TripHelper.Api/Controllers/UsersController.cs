using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripHelper.Application.Users.Commands.CreateUser;
using TripHelper.Application.Users.Commands.DeleteUser;
using TripHelper.Application.Users.Queries.GetUser;
using TripHelper.Contracts.Users;

namespace TripHelper.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class UsersController(
    ISender sender
) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Firstname, request.Lastname, request.Password, request.Email, request.IsSuperAdmin);

        var result = await _sender.Send(command);

        return result.Match(
            user => Ok(new UserResponse(user.Id, user.Email, user.Firstname, user.Lastname, user.IsSuperAdmin)),
            Problem);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var query = new GetUserQuery(id);

        var result = await _sender.Send(query);

        return result.Match(
            user => Ok(new UserResponse(user.Id, user.Email, user.Firstname, user.Lastname, user.IsSuperAdmin)),
            Problem);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var command = new DeleteUserCommand(id);

        var result = await _sender.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}