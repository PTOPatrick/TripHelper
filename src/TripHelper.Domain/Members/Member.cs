using TripHelper.Domain.Common;
using TripHelper.Domain.Members.Events;

namespace TripHelper.Domain.Members;

public class Member : Entity
{
    public int UserId { get; private set; }
    public int TripId { get; private set; }
    public bool IsAdmin { get; private set; }

    public Member(int userId, int tripId, bool isAdmin = false)
    {
        UserId = userId;
        TripId = tripId;
        IsAdmin = isAdmin;
    }

    public void Update(bool isAdmin)
    {
        IsAdmin = isAdmin;
    }

    public void MemberDeleted()
    {
        _domainEvents.Add(new MemberDeletedEvent(Id, TripId, UserId));
    }

    private Member() { }
}