using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.DeleteTrip;

public class DeleteTripCommandHandler(
    ITripsRepository _tripsRepository,
    IUnitOfWork _unitOfWork,
    IAuthorizationService _authorizationService
) : IRequestHandler<DeleteTripCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanDeleteTrip(request.TripId))
            return Error.Unauthorized();

        var trip = await _tripsRepository.GetTripByIdAsync(request.TripId);
        if (trip is null)
            return TripErrors.TripNotFound;

        trip.DeleteTrip();

        await _tripsRepository.DeleteTripAsync(trip);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}