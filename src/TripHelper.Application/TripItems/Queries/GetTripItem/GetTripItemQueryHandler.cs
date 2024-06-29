using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.TripItems;

namespace TripHelper.Application.TripItems.Queries.GetTripItem;

public class GetTripItemQueryHandler(
    ITripItemsRepository _tripItemsRepository,
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    IAuthorizationService _authorizationService
) : IRequestHandler<GetTripItemQuery, ErrorOr<TripItemWithEmail>>
{
    public async Task<ErrorOr<TripItemWithEmail>> Handle(GetTripItemQuery request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanGetTripItem(request.TripId))
            return Error.Unauthorized();

        var tripItem = await _tripItemsRepository.GetTripItemAsync(request.TripItemId);
        if (tripItem is null)
            return TripItemErrors.TripItemNotFound;

        var member = await _membersRepository.GetMemberAsync(tripItem.MemberId);
        if (member is null)
            return TripItemErrors.MemberNotFound;

        var user = await _usersRepository.GetUserByIdAsync(member.UserId);
        if (user is null)
            return TripItemErrors.UserNotFound;

        return new TripItemWithEmail(
            tripItem.Id,
            tripItem.Name,
            tripItem.Amount,
            tripItem.MemberId,
            user.Email
        );
    }
}