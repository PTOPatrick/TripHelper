using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Application.Common.Services.Authorization;

namespace TripHelper.Application.TripItems.Queries.GetTripItems;

public class GetTripItemsQueryHandler(
    ITripItemsRepository _tripItemsRepository,
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    IAuthorizationService _authorizationService) : IRequestHandler<GetTripItemsQuery, ErrorOr<List<TripItemWithEmail>>>
{
    public async Task<ErrorOr<List<TripItemWithEmail>>> Handle(GetTripItemsQuery request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanGetTripItem(request.TripId))
            return Error.Unauthorized();

        var tripItems = await _tripItemsRepository.GetTripItemsByTripIdAsync(request.TripId);
        var members = await _membersRepository.GetMembersByTripIdAsync(request.TripId);
        var users = await _usersRepository.GetUsersByIdsAsync(members.Select(m => m.UserId).ToList());

        return tripItems.Select(ti => new TripItemWithEmail(ti.Id, ti.Name, ti.Amount, ti.MemberId, users.First(u => u.Id == ti.MemberId).Email)).ToList();
    }
}