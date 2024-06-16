using TripHelper.Domain.Common;

namespace TripHelper.Domain.Trips.Events;

public record TripDeletedEvent(int TripId) : IDomainEvent;