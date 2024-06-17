using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.UpdateTrip;

[Authorize]
public record UpdateTripCommand(int TripId, string Name, DateTime? StartDate, DateTime? EndDate, string? Description, string? Location, string? ImageUrl) : IRequest<ErrorOr<Trip>>;