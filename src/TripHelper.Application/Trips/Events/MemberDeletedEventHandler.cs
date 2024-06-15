using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members.Events;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Events;

public class MemberDeletedEventHandler(
    ITripsRepository tripsRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<MemberDeletedEvent>
{
    private readonly ITripsRepository _tripsRepository = tripsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private Trip? _trip;

    public async Task Handle(MemberDeletedEvent notification, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateRequestAndFillUpTrip(notification);
        if (validationResult.IsError)
            return;

        await UpdateOrDeleteTrip();
    }

    private async Task UpdateOrDeleteTrip()
    {
        if (!TripHasMembers())
            await _tripsRepository.DeleteTripAsync(_trip!);
        else
            await _tripsRepository.UpdateTripAsync(_trip!);

        await _unitOfWork.CommitChangesAsync();
    }

    private bool TripHasMembers() => _trip!.GetMemberCount() is not 0;

    private async Task<ErrorOr<Success>> ValidateRequestAndFillUpTrip(MemberDeletedEvent notification)
    {
        _trip = await _tripsRepository.GetTripByIdAsync(notification.TripId);
        if (_trip is null)
            return TripErrors.TripNotFound;

        var result = _trip.RemoveMember(notification.MemberId);
        if (result.IsError)
            return result.Errors;

        return Result.Success;
    }
}