using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.UpdateTrip;

public class UpdateTripCommandHandler(
    ITripsRepository tripsRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateTripCommand, ErrorOr<Trip>>
{
    private readonly ITripsRepository _tripsRepository = tripsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Trip>> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripsRepository.GetTripByIdAsync(request.TripId);
        if (trip is null)
            return TripErrors.TripNotFound;

        await _tripsRepository.UpdateTripAsync(trip);
        await _unitOfWork.CommitChangesAsync();

        return trip;
    }
}