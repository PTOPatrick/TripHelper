using ErrorOr;
namespace TripHelper.Domain.Members;

public static class MemberErrors
{
    public static readonly Error MemberNotFound = Error.NotFound(
        code: "Member.MemberNotFound",
        description: "Member is not found.");
}