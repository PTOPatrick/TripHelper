using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.DeleteTrip;

public class DeleteTripCommandHandler(
    ITripsRepository tripsRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteTripCommand, ErrorOr<Deleted>>
{
    private readonly ITripsRepository _tripsRepository = tripsRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Deleted>> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripsRepository.GetTripByIdAsync(request.TripId);
        if (trip is null)
            return TripErrors.TripNotFound;
        
        trip.DeleteTrip();

        await _tripsRepository.DeleteTripAsync(trip);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}