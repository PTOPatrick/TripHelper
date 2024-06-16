using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Queries.GetTrips;

public class GetTripsQueryHandler(
    IMembersRepository membersRepository,
    ITripsRepository tripsRepository
) : IRequestHandler<GetTripsQuery, ErrorOr<List<Trip>>>
{
    private readonly IMembersRepository _membersRepository = membersRepository;
    private readonly ITripsRepository _tripsRepository = tripsRepository;

    public async Task<ErrorOr<List<Trip>>> Handle(GetTripsQuery request, CancellationToken cancellationToken)
    {
        var members = await _membersRepository.GetMembersByUserIdAsync(request.UserId);

        if (members.Count is 0)
            return TripErrors.UserHasNoTrips;

        var trips = await _tripsRepository.GetTripsByIdsAsync(members.Select(m => m.TripId).ToList());
        if (trips.Count is 0)
            return TripErrors.TripsNotFound;

        return trips;
    }
}