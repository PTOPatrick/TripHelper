using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.TripItems;

namespace TripHelper.Application.TripItems.Commands.DeleteTripItem;

public class DeleteTripItemCommandHandler(
    ITripItemsRepository _tripItemsRepository,
    IUnitOfWork _unitOfWork,
    IAuthorizationService _authorizationService
) : IRequestHandler<DeleteTripItemCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteTripItemCommand request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanDeleteTripItem(request.TripId))
            return Error.Unauthorized();

        var tripItem = await _tripItemsRepository.GetTripItemAsync(request.TripItemId);
        if (tripItem is null)
            return TripItemErrors.TripItemNotFound;

        await _tripItemsRepository.DeleteTripItemAsync(tripItem.Id);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}