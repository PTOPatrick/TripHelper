using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Queries.GetTrips;

[Authorize]
public record GetTripsQuery() : IRequest<ErrorOr<List<Trip>>>;