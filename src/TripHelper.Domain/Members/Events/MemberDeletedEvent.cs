using TripHelper.Domain.Common;

namespace TripHelper.Domain.Members.Events;

public record MemberDeletedEvent(int MemberId, int TripId, int UserId) : IDomainEvent;