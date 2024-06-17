using ErrorOr;
namespace TripHelper.Domain.Members;

public static class MemberErrors
{
    public static readonly Error MemberNotFound = Error.NotFound(
        code: "Member.MemberNotFound",
        description: "Member is not found.");
        
    public static readonly Error UserNotFound = Error.NotFound(
        code: "Member.UserNotFound",
        description: "User is not found.");
        
    public static readonly Error TripNotFound = Error.NotFound(
        code: "Member.TripNotFound",
        description: "Trip is not found.");

    public static readonly Error MemberAlreadyExists = Error.NotFound(
        code: "Member.MemberAlreadyExists",
        description: "Member already exists.");
}