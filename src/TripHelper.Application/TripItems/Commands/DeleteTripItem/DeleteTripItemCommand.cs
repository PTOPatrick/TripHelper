using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;

namespace TripHelper.Application.TripItems.Commands.DeleteTripItem;

[Authorize]
public record DeleteTripItemCommand(int TripId, int TripItemId) : IRequest<ErrorOr<Deleted>>;