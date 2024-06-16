using ErrorOr;
using MediatR;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Commands.CreateTrip;

public record CreateTripCommand(string Name, DateTime? StartDate, DateTime? EndDate, string? Description, string? Location, string? ImageUrl) : IRequest<ErrorOr<Trip>>;