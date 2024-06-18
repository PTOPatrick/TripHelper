using TripHelper.Domain.Users;

namespace TestCommon.Users;

public static class UserFactory
{
    public static User CreateUser(string email, string firstname, string lastname, string password, bool isSuperAdmin)
    {
        return new User(email, firstname, lastname, password, isSuperAdmin);
    }
}