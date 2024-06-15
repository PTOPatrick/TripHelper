using TripHelper.Domain.Common;

namespace TripHelper.Domain.Users.Events;

public record MemberDeletedEvent(int MemberId) : IDomainEvent;