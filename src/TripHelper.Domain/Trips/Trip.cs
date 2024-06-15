using ErrorOr;

namespace TripHelper.Domain.Trips;

public class Trip
{
    private readonly List<int> _memberIds = [];
    
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string Description { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public string ImageUrl { get; private set; } = null!;
    public int CreatorUserId { get; private set; }

    public Trip(string name, DateTime? startDate, DateTime? endDate, string description, string location, string imageUrl, int creatorUserId)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
        Location = location;
        ImageUrl = imageUrl;
        CreatorUserId = creatorUserId;
    }

    public ErrorOr<Success> RemoveMember(int memberId)
    {
        if (!_memberIds.Contains(memberId))
            return TripErrors.MemberNotFound;
        
        _memberIds.Remove(memberId);

        return Result.Success;
    }

    public int GetMemberCount() => _memberIds.Count;

    private Trip() { }
}