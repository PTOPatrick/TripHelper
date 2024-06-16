using ErrorOr;
using MediatR;

namespace TripHelper.Application.Trips.Commands.DeleteTrip;

public record DeleteTripCommand(int TripId) : IRequest<ErrorOr<Deleted>>;