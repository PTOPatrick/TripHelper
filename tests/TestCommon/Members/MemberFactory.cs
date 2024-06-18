using TripHelper.Domain.Members;

namespace TestCommon.Members;

public static class MemberFactory
{
    public static Member CreateMember(int userId, int tripId)
    {
        return new Member(userId, tripId);
    }
}