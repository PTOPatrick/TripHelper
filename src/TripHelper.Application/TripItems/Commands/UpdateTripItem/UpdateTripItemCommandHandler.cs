using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.TripItems;

namespace TripHelper.Application.TripItems.Commands.UpdateTripItem;

public class UpdateTripItemCommandHandler(
    ITripItemsRepository _tripItemsRepository,
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    IAuthorizationService _authorizationService) : IRequestHandler<UpdateTripItemCommand, ErrorOr<TripItemWithEmail>>
{
    public async Task<ErrorOr<TripItemWithEmail>> Handle(UpdateTripItemCommand request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanUpdateTripItem(request.TripId))
            return Error.Unauthorized();

        var tripItem = await _tripItemsRepository.GetTripItemAsync(request.TripItemId);
        if (tripItem is null)
            return TripItemErrors.TripItemNotFound;

        var member = await _membersRepository.GetMemberAsync(request.MemberId);
        if (member is null)
            return TripItemErrors.MemberNotFound;

        var user = await _usersRepository.GetUserByIdAsync(member.UserId);
        if (user is null)
            return TripItemErrors.UserNotFound;

        tripItem.Update(request.Name, request.Amount, request.MemberId);

        await _tripItemsRepository.UpdateTripItemAsync(tripItem);
        await _unitOfWork.CommitChangesAsync();

        return new TripItemWithEmail(
            tripItem.Id,
            tripItem.Name,
            tripItem.Amount,
            tripItem.MemberId,
            user.Email
        );
    }
}