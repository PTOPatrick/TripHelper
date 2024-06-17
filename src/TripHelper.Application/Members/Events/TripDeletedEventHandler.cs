using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips.Events;

namespace TripHelper.Application.Members.Events;

public class TripDeletedEventHandler(
    IMembersRepository _membersRepository,
    IUnitOfWork _unitOfWork
) : INotificationHandler<TripDeletedEvent>
{
    public async Task Handle(TripDeletedEvent notification, CancellationToken cancellationToken)
    {
        var members = await _membersRepository.GetMembersByTripIdAsync(notification.TripId);

        await _membersRepository.DeleteMembersAsync(members);
        await _unitOfWork.CommitChangesAsync();
    }
}