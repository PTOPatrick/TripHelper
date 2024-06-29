using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Queries.GetTrips;

public class GetTripsQueryHandler(
    IMembersRepository _membersRepository,
    ITripsRepository _tripsRepository,
    IAuthorizationService _authorizationService
) : IRequestHandler<GetTripsQuery, ErrorOr<List<Trip>>>
{
    public async Task<ErrorOr<List<Trip>>> Handle(GetTripsQuery request, CancellationToken cancellationToken)
    {
        if (_authorizationService.IsSuperAdmin())
            return await _tripsRepository.GetTripsAsync();

        var members = await _membersRepository.GetMembersByUserIdAsync(_authorizationService.GetCurrentUserId());

        if (members.Count is 0)
            return TripErrors.UserHasNoTrips;

        var trips = await _tripsRepository.GetTripsByIdsAsync(members.Select(m => m.TripId).ToList());
        if (trips.Count is 0)
            return TripErrors.TripsNotFound;

        return trips;
    }
}