using ErrorOr;
namespace TripHelper.Domain.TripItems;

public static class TripItemErrors
{
    public static readonly Error AmountMustBePositive = Error.NotFound(
        code: "TripItem.AmountMustBePositive",
        description: "Amount must be positive.");

    public static readonly Error TripItemNotFound = Error.NotFound(
        code: "TripItem.TripItemNotFound",
        description: "Trip item not found.");
        
    public static readonly Error MemberNotFound = Error.NotFound(
        code: "TripItem.MemberNotFound",
        description: "Member not found.");
        
    public static readonly Error UserNotFound = Error.NotFound(
        code: "TripItem.UserNotFound",
        description: "User not found.");
}