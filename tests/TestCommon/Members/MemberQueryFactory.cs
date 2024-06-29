using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Members.Queries.GetMember;
using TripHelper.Application.Members.Queries.GetTripMembers;

namespace TestCommon.Members
{
    public static class MemberQueryFactory
    {
        public static GetMemberQuery CreateGetMemberQuery(int id)
        {
            return new GetMemberQuery(id);
        }

        public static GetTripMembersQuery CreateGetTripMembersQuery(int tripId)
        {
            return new GetTripMembersQuery(tripId);
        }

        public static GetMemberQueryHandler CreateGetMemberQueryHandler(
            IMembersRepository MembersRepository,
            IUsersRepository usersRepository,
            IAuthorizationService authorizationService)
        {
            return new GetMemberQueryHandler(MembersRepository, usersRepository, authorizationService);
        }

        public static GetTripMembersQueryHandler CreateGetTripMembersQueryHandler(
            IMembersRepository MembersRepository,
            IUsersRepository usersRepository,
            IAuthorizationService authorizationService)
        {
            return new GetTripMembersQueryHandler(MembersRepository, usersRepository, authorizationService);
        }
    }
}