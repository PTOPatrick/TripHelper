using ErrorOr;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Common.Interfaces;

public interface ITripsRepository
{
    Task AddTripAsync(Trip trip);
    Task DeleteTripAsync(Trip trip);
    Task<Trip?> GetTripByIdAsync(int id);
    Task<List<Trip>> GetTripsByIdsAsync(List<int> ids);
    Task UpdateTripAsync(Trip trip);
}