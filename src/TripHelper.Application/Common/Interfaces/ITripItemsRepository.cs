using TripHelper.Domain.TripItems;

namespace TripHelper.Application.Common.Interfaces;

public interface ITripItemsRepository
{
    Task<TripItem?> GetTripItemAsync(int id);
    Task<List<TripItem>> GetTripItemsAsync(int tripId);
    Task AddTripItemAsync(TripItem tripItem);
    Task UpdateTripItemAsync(TripItem tripItem);
    Task DeleteTripItemAsync(int id);
    Task<List<TripItem>> GetTripItemsByTripIdAsync(int tripId);
    Task DeleteRangeTripItemAsync(List<TripItem> tripItems);
}