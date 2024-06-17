using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.TripItems.Queries.GetTripItems;

[Authorize]
public record GetTripItemsQuery(int TripId) : IRequest<ErrorOr<List<TripItemWithEmail>>>;