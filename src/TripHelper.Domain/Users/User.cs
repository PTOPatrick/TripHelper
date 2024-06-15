using ErrorOr;
using TripHelper.Domain.Common;
using TripHelper.Domain.Members;
using TripHelper.Domain.Users.Events;

namespace TripHelper.Domain.Users;

public class User : Entity
{
    private readonly List<int> _memberIds = [];
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

    public ErrorOr<Success> AddMember(Member member)
    {
        if (_memberIds.Contains(member.Id))
            return UserErrors.MemberAlreadyAdded;

        if (_memberIds.Count >= _maxTrips)
            return UserErrors.MaxTripsReached;
        
        _memberIds.Add(member.Id);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveMember(Member member)
    {
        if (!_memberIds.Contains(member.Id))
            return UserErrors.MemberNotFound;
        
        _memberIds.Remove(member.Id);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveMember(int memberId)
    {
        if (!_memberIds.Contains(memberId))
            return UserErrors.MemberNotFound;
        
        _memberIds.Remove(memberId);
        return Result.Success;
    }

    public ErrorOr<Success> DeleteUser()
    {
        _domainEvents.Add(new UserDeletedEvent(Id));

        return Result.Success;
    }

    public ErrorOr<Success> DeleteMember(int memberId)
    {
        var removeResult = RemoveMember(memberId);
        if (removeResult.IsError)
            return removeResult.Errors;
        
        _domainEvents.Add(new MemberDeletedEvent(memberId));

        return Result.Success;
    }

    private int GetMaxTrips() => IsSuperAdmin ? 100 : 10;

    private User() { }
}