using TripHelper.Domain.Trips;

namespace TripHelper.Application.Common.Interfaces;

public interface ITripsRepository
{
    Task AddTripAsync(Trip trip);
    Task DeleteTripAsync(Trip trip);
    Task<Trip?> GetTripByIdAsync(int id);
    Task UpdateTripAsync(Trip trip);
}