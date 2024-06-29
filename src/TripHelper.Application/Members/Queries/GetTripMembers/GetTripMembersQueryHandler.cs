using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Members.Queries.GetTripMembers;

public class GetTripMembersQueryHandler(
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    IAuthorizationService _authorizationService
) : IRequestHandler<GetTripMembersQuery, ErrorOr<List<MemberWithEmail>>>
{
    public async Task<ErrorOr<List<MemberWithEmail>>> Handle(GetTripMembersQuery request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanGetTrip(request.TripId))
            return Error.Unauthorized();

        var members = await _membersRepository.GetMembersByTripIdAsync(request.TripId);
        var users = await _usersRepository.GetUsersByIdsAsync(members.Select(m => m.UserId).ToList());

        return members.Select(m => new MemberWithEmail(m.Id, m.UserId, m.TripId, m.IsAdmin, users.First(u => u.Id == m.UserId).Email)).ToList();
    }
}