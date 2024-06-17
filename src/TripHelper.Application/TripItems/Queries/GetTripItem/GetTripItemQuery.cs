using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.TripItems.Queries.GetTripItem;

[Authorize]
public record GetTripItemQuery(int TripId, int TripItemId) : IRequest<ErrorOr<TripItemWithEmail>>;