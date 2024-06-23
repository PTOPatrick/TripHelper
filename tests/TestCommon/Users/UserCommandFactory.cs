using TripHelper.Application.Users.Commands.CreateUser;
using TripHelper.Application.Users.Commands.DeleteUser;

namespace TestCommon.Users
{
    public static class UserCommandFactory
    {
        public static CreateUserCommand CreateCreateUserCommand(string firstname, string lastname, string password, string email, bool isSuperAdmin)
        {
            return new CreateUserCommand(firstname, lastname, password, email, isSuperAdmin);
        }

        public static DeleteUserCommand CreateDeleteUserCommand(int userId)
        {
            return new DeleteUserCommand(userId);
        }
    }
}