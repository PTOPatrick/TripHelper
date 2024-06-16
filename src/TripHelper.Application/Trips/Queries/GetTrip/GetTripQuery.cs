using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Queries.GetTrip;

public record GetTripQuery(int TripId) : IRequest<ErrorOr<TripWithEmails>>;