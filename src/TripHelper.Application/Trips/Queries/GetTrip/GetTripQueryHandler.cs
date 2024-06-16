using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Queries.GetTrip;

public class GetTripQueryHandler(
    ITripsRepository tripsRepository,
    IMembersRepository membersRepository,
    IUsersRepository usersRepository) : IRequestHandler<GetTripQuery, ErrorOr<TripWithEmails>>
{
    private readonly ITripsRepository _tripsRepository = tripsRepository;
    private readonly IMembersRepository _membersRepository = membersRepository;
    private readonly IUsersRepository _usersRepository = usersRepository;

    public async Task<ErrorOr<TripWithEmails>> Handle(GetTripQuery request, CancellationToken cancellationToken)
    {
        var trip = await _tripsRepository.GetTripByIdAsync(request.TripId);
        if (trip is null)
            return TripErrors.TripNotFound;

        var members = await _membersRepository.GetMembersByTripIdAsync(request.TripId);
        if (members.Count is 0)
            return TripWithEmails.FromTrip(trip, []);

        var users = await _usersRepository.GetUsersByIdsAsync(members.Select(m => m.UserId).ToList());

        return TripWithEmails.FromTrip(trip, users.Select(u => u.Email).ToList());
    }
}