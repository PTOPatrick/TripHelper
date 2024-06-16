using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips.Events;

namespace TripHelper.Application.Members.Events;

public class TripDeletedEventHandler(
    IMembersRepository membersRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<TripDeletedEvent>
{
    private readonly IMembersRepository _membersRepository = membersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(TripDeletedEvent notification, CancellationToken cancellationToken)
    {
        var members = await _membersRepository.GetMembersByTripIdAsync(notification.TripId);

        await _membersRepository.DeleteMembersAsync(members);
        await _unitOfWork.CommitChangesAsync();
    }
}