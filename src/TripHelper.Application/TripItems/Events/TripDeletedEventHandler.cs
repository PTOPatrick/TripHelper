using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips.Events;

namespace TripHelper.Application.TripItems.Events;

public class TripDeletedEventHandler(
    ITripItemsRepository _tripItemsRepository,
    IUnitOfWork _unitOfWork
) : INotificationHandler<TripDeletedEvent>
{
    public async Task Handle(TripDeletedEvent notification, CancellationToken cancellationToken)
    {
        var tripItems = await _tripItemsRepository.GetTripItemsByTripIdAsync(notification.TripId);

        await _tripItemsRepository.DeleteRangeTripItemAsync(tripItems);
        await _unitOfWork.CommitChangesAsync();
    }
}