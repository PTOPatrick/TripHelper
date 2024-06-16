using ErrorOr;
using TripHelper.Domain.Common;
using TripHelper.Domain.Users.Events;

namespace TripHelper.Domain.Users;

public class User : Entity
{
    private readonly int _maxTrips;

    public string Email { get; private set; } = null!;
    public string Firstname { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string Password { get; private set; } = null!;
    public bool IsSuperAdmin { get; private set; } = false;

    public User(string email, string firstname, string lastname, string password, bool isSuperAdmin)
    {
        Email = email;
        Firstname = firstname;
        Lastname = lastname;
        Password = password;
        IsSuperAdmin = isSuperAdmin;
        _maxTrips = GetMaxTrips();
    }

    public ErrorOr<Success> DeleteUser()
    {
        _domainEvents.Add(new UserDeletedEvent(Id));

        return Result.Success;
    }

    public ErrorOr<Success> DeleteMember(int memberId)
    {
        _domainEvents.Add(new MemberDeletedEvent(memberId));

        return Result.Success;
    }

    private int GetMaxTrips() => IsSuperAdmin ? 100 : 10;

    private User() { }
}