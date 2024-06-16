using Microsoft.EntityFrameworkCore;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Infrastructure.Common.Persistence;

namespace TripHelper.Infrastructure.Trip.Persistence
{
    public class TripsRepository(TripHelperDbContext dbContext) : ITripsRepository
    {
        private readonly TripHelperDbContext _dbContext = dbContext;
        
        public async Task AddTripAsync(Domain.Trips.Trip trip)
        {
            await _dbContext.AddAsync(trip);
        }

        public async Task<Domain.Trips.Trip?> GetTripByIdAsync(int id)
        {
            return await _dbContext.Trips.FindAsync(id);
        }

        public async Task UpdateTripAsync(Domain.Trips.Trip trip)
        {
            _dbContext.Trips.Update(trip);

            await Task.CompletedTask;
        }

        public async Task DeleteTripAsync(Domain.Trips.Trip trip)
        {
            _dbContext.Trips.Remove(trip);

            await Task.CompletedTask;
        }

        public async Task<List<Domain.Trips.Trip>> GetTripsByIdsAsync(List<int> ids)
        {
            return await _dbContext.Trips.Where(t => ids.Contains(t.Id)).ToListAsync();
        }
    }
}