using TripHelper.Domain.Common;

namespace TripHelper.Domain.Users.Events;

public record UserDeletedEvent(int UserId) : IDomainEvent;