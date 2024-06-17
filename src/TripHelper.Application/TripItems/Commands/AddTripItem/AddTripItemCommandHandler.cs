using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.TripItems;

namespace TripHelper.Application.TripItems.Commands.AddTripItem;

public class CreateTripItemCommandHandler(
    ITripItemsRepository _tripItemsRepository,
    IUnitOfWork _unitOfWork,
    AuthorizationService _authorizationService
) : IRequestHandler<CreateTripItemCommand, ErrorOr<TripItem>>
{
    public async Task<ErrorOr<TripItem>> Handle(CreateTripItemCommand request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanCreateTripItem(request.TripId))
            return Error.Unauthorized();
        
        var tripItem = new TripItem(request.Name, request.TripId, request.MemberId);

        var result = tripItem.AssignAmount(request.Amount);
        if (result.IsError)
            return result.Errors;

        await _tripItemsRepository.AddTripItemAsync(tripItem);
        await _unitOfWork.CommitChangesAsync();
        
        return tripItem;
    }
}