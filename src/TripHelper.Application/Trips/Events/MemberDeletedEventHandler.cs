using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members.Events;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Events;

public class MemberDeletedEventHandler(
    ITripsRepository tripsRepository,
    IMembersRepository membersRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<MemberDeletedEvent>
{
    private readonly ITripsRepository _tripsRepository = tripsRepository;
    private readonly IMembersRepository _membersRepository = membersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private Trip? _trip;

    public async Task Handle(MemberDeletedEvent notification, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateRequestAndFillUpTrip(notification);
        if (validationResult.IsError)
            return;

        await DeleteTripIfNoMembersAssigned();
    }

    private async Task DeleteTripIfNoMembersAssigned()
    {
        if (!await TripHasMembers())
        {
            _trip!.DeleteTrip();
            await _tripsRepository.DeleteTripAsync(_trip!);
        }

        await _unitOfWork.CommitChangesAsync();
    }

    private async Task<bool> TripHasMembers()
    {
        var members = await _membersRepository.GetMembersByTripIdAsync(_trip!.Id);
        return members.Count is not 0;
    }

    private async Task<ErrorOr<Success>> ValidateRequestAndFillUpTrip(MemberDeletedEvent notification)
    {
        _trip = await _tripsRepository.GetTripByIdAsync(notification.TripId);
        if (_trip is null)
            return TripErrors.TripNotFound;

        return Result.Success;
    }
}