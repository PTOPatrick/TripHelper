using TripHelper.Domain.Common;
using TripHelper.Domain.Trips.Events;

namespace TripHelper.Domain.Trips;

public class Trip : Entity
{
    public string Name { get; private set; } = null!;
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public string? ImageUrl { get; private set; }
    public int CreatorUserId { get; private set; }

    public Trip(string name, DateTime? startDate, DateTime? endDate, string? description, string? location, string? imageUrl, int creatorUserId)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        Location = location;
        ImageUrl = imageUrl;
        CreatorUserId = creatorUserId;
    }

    public void DeleteTrip()
    {
        _domainEvents.Add(new TripDeletedEvent(Id));
    }

    public void Update(string name, DateTime? startDate, DateTime? endDate, string? description, string? location, string? imageUrl)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        Location = location;
        ImageUrl = imageUrl;
    }

    public void CreateTrip()
    {
        _domainEvents.Add(new TripCreatedEvent(Id));
    }

    private Trip() { }
}