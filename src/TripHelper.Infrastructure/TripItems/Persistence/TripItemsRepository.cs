using Microsoft.EntityFrameworkCore;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.TripItems;
using TripHelper.Infrastructure.Common.Persistence;

namespace TripHelper.Infrastructure.TripItems.Persistence;

public class TripItemsRepository(TripHelperDbContext _dbContext) : ITripItemsRepository
{
    public async Task AddTripItemAsync(TripItem tripItem)
    {
        await _dbContext.TripItems.AddAsync(tripItem);
    }

    public Task DeleteTripItemAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<TripItem?> GetTripItemAsync(int id)
    {
        return await _dbContext.TripItems.FindAsync(id);
    }

    public Task<List<TripItem>> GetTripItemsAsync(int tripId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTripItemAsync(TripItem tripItem)
    {
        throw new NotImplementedException();
    }

    public async Task<List<TripItem>> GetTripItemsByTripIdAsync(int tripId)
    {
        return await _dbContext.TripItems.Where(ti => ti.TripId == tripId).ToListAsync();
    }

    public Task DeleteRangeTripItemAsync(List<TripItem> tripItems)
    {
        _dbContext.TripItems.RemoveRange(tripItems);

        return Task.CompletedTask;
    }
}
