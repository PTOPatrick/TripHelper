using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.UpdateTrip;

public class UpdateTripCommandHandler(
    ITripsRepository _tripsRepository,
    IUnitOfWork _unitOfWork,
    AuthorizationService _authorizationService
) : IRequestHandler<UpdateTripCommand, ErrorOr<Trip>>
{
    public async Task<ErrorOr<Trip>> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanUpdateTrip(request.TripId))
            return Error.Unauthorized();
            
        var trip = await _tripsRepository.GetTripByIdAsync(request.TripId);
        if (trip is null)
            return TripErrors.TripNotFound;

        trip.Update(request.Name, request.StartDate, request.EndDate, request.Description, request.Location, request.ImageUrl);

        await _tripsRepository.UpdateTripAsync(trip);
        await _unitOfWork.CommitChangesAsync();

        return trip;
    }
}