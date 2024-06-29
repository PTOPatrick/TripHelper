using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Users.Queries.GetUser;

namespace TestCommon.Users
{
    public static class UserQueryFactory
    {
        public static GetUserQuery CreateGetUserQuery(int id)
        {
            return new GetUserQuery(id);
        }

        public static GetUserQueryHandler CreateGetUserQueryHandler(
            IUsersRepository usersRepository, 
            IAuthorizationService authorizationService)
        {
            return new GetUserQueryHandler(usersRepository, authorizationService);
        }
    }
}