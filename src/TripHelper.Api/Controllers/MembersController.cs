using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripHelper.Application.Members.Commands.CreateMember;
using TripHelper.Application.Members.Commands.DeleteMember;
using TripHelper.Application.Members.Commands.UpdateMember;
using TripHelper.Application.Members.Queries.GetMember;
using TripHelper.Contracts.Members;

namespace TripHelper.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class MembersController(
    ISender sender
) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateMember(CreateMemberRequest request)
    {
        var command = new CreateMemberCommand(request.Email, request.TripId, request.IsAdmin);

        var result = await _sender.Send(command);

        return result.Match(
            member => Ok(new MemberResponse(member.Id, member.UserId, member.TripId, member.IsAdmin, member.Email)),
            Problem);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMember(int id)
    {
        var query = new GetMemberQuery(id);

        var result = await _sender.Send(query);

        return result.Match(
            member => Ok(new MemberResponse(member.Id, member.UserId, member.TripId, member.IsAdmin, member.Email)),
            Problem);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMember(int id, UpdateMemberRequest request)
    {
        var command = new UpdateMemberCommand(id, request.IsAdmin);

        var result = await _sender.Send(command);

        return result.Match(
            member => Ok(new MemberResponse(member.Id, member.UserId, member.TripId, member.IsAdmin, member.Email)),
            Problem);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMember(int id)
    {
        var command = new DeleteMemberCommand(id);

        var result = await _sender.Send(command);

        return result.Match(
            _ => Ok(),
            Problem);
    }
}