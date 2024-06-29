using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Trips.Queries.GetTrip;

public class GetTripQueryHandler(
    ITripsRepository _tripsRepository,
    IMembersRepository _membersRepository,
    IAuthorizationService _authorizationService,
    IUsersRepository _usersRepository) : IRequestHandler<GetTripQuery, ErrorOr<TripWithEmails>>
{
    public async Task<ErrorOr<TripWithEmails>> Handle(GetTripQuery request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanGetTrip(request.TripId))
            return Error.Unauthorized();

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