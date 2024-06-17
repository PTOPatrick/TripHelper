using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Trips.Queries.GetTrip;

[Authorize]
public record GetTripQuery(int TripId) : IRequest<ErrorOr<TripWithEmails>>;