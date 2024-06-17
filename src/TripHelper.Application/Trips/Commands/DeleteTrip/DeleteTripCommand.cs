using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;

namespace TripHelper.Application.Trips.Commands.DeleteTrip;

[Authorize]
public record DeleteTripCommand(int TripId) : IRequest<ErrorOr<Deleted>>;