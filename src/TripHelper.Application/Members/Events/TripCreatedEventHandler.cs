using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;
using TripHelper.Domain.Trips.Events;

namespace TripHelper.Application.Members.Events;

public class TripCreatedEventHandler(
    ITripsRepository _tripRepository,
    IMembersRepository _membersRepository,
    IUnitOfWork _unitOfWork
) : INotificationHandler<TripCreatedEvent>
{
    public async Task Handle(TripCreatedEvent notification, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetTripByIdAsync(notification.TripId);
        if (trip is null)
            return;
        
        var member = new Member(trip.CreatorUserId, trip.Id, true);
        
        await _membersRepository.AddMemberAsync(member);
        await _unitOfWork.CommitChangesAsync();
    }
}