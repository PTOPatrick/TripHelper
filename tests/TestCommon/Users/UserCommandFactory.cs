using TripHelper.Application.Common.Interfaces;
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

        public static DeleteUserCommandHandler CreateDeleteUserCommandHandler(
            IUsersRepository usersRepository, 
            IUnitOfWork unitOfWork, 
            IAuthorizationService authorizationService)
        {
            return new DeleteUserCommandHandler(usersRepository, unitOfWork, authorizationService);
        }
    }
}