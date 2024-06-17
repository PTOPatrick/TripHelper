using ErrorOr;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TripHelper.Domain.Trips;

namespace TripHelper.Application.Common.Interfaces;

public interface ITripsRepository
{
    Task<EntityEntry<Trip>> AddTripAsync(Trip trip);
    Task DeleteTripAsync(Trip trip);
    Task<Trip?> GetTripByIdAsync(int id);
    Task<List<Trip>> GetTripsAsync();
    Task<List<Trip>> GetTripsByIdsAsync(List<int> ids);
    Task UpdateTripAsync(Trip trip);
}