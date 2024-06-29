using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.CreateTrip;

public class CreateTripCommandHandler(
    ITripsRepository _tripRepository,
    IUnitOfWork _unitOfWork,
    IAuthorizationService _authorizationService
) : IRequestHandler<CreateTripCommand, ErrorOr<Trip>>
{
    public async Task<ErrorOr<Trip>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        return await CreateTripFromRequest(request);
    }

    private async Task<Trip> CreateTripFromRequest(CreateTripCommand request)
    {
        var trip = new Trip(
            request.Name,
            request.StartDate,
            request.EndDate,
            request.Description,
            request.Location,
            request.ImageUrl,
            _authorizationService.GetCurrentUserId());

        await _tripRepository.AddTripAsync(trip);
        await _unitOfWork.CommitChangesAsync();

        trip.CreateTrip();
        await _unitOfWork.CommitChangesAsync();

        return trip;
    }
}