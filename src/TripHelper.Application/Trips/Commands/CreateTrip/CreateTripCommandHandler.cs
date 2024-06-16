using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.CreateTrip;

public class CreateTripCommandHandler(
    ITripsRepository _tripRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateTripCommand, ErrorOr<Trip>>
{
    private readonly ITripsRepository _tripRepository = _tripRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Trip>> Handle(CreateTripCommand request, CancellationToken cancellationToken)
    {
        return await CreateTripFromRequest(request);
    }

    private async Task<Trip> CreateTripFromRequest(CreateTripCommand request)
    {
        var trip = new Trip(request.Name, request.StartDate, request.EndDate, request.Description, request.Location, request.ImageUrl, 1);

        await _tripRepository.AddTripAsync(trip);
        await _unitOfWork.CommitChangesAsync();

        return trip;
    }
}