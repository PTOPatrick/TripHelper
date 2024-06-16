using ErrorOr;
using MediatR;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Queries.GetTrips;

public record GetTripsQuery(int UserId) : IRequest<ErrorOr<List<Trip>>>;