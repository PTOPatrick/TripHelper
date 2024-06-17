using TripHelper.Domain.Common;

namespace TripHelper.Domain.Trips.Events;

public record TripCreatedEvent(int TripId) : IDomainEvent;