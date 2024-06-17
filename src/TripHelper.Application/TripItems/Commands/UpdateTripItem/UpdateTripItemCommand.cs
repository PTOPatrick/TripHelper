using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.TripItems.Commands.UpdateTripItem;

[Authorize]
public record UpdateTripItemCommand(int TripId, int TripItemId, string Name, decimal Amount, int MemberId) : IRequest<ErrorOr<TripItemWithEmail>>;