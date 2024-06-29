using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Domain.TripItems;

namespace TripHelper.Application.TripItems.Commands.CreateTripItem;

[Authorize]
public record CreateTripItemCommand(string Name, int TripId, decimal Amount, int MemberId) : IRequest<ErrorOr<TripItem>>;