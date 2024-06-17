using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;
using TripHelper.Infrastructure.Common.Persistence;

namespace TripHelper.Infrastructure.Trips.Persistence;

public class TripsRepository(TripHelperDbContext _dbContext) : ITripsRepository
{
    public async Task<EntityEntry<Trip>> AddTripAsync(Trip trip)
    {
        return await _dbContext.AddAsync(trip);
    }

    public async Task<Trip?> GetTripByIdAsync(int id)
    {
        return await _dbContext.Trips.FindAsync(id);
    }

    public async Task UpdateTripAsync(Trip trip)
    {
        _dbContext.Trips.Update(trip);

        await Task.CompletedTask;
    }

    public async Task DeleteTripAsync(Trip trip)
    {
        _dbContext.Trips.Remove(trip);

        await Task.CompletedTask;
    }

    public async Task<List<Trip>> GetTripsByIdsAsync(List<int> ids)
    {
        return await _dbContext.Trips.Where(t => ids.Contains(t.Id)).ToListAsync();
    }

    public Task<List<Trip>> GetTripsAsync()
    {
        return _dbContext.Trips.ToListAsync();
    }
}