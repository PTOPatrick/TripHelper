using ErrorOr;
namespace TripHelper.Domain.Users;

public static class UserErrors
{
    public static readonly Error CannotCreateUserWithEmptyEmail = Error.Validation(
        code: "User.CanNotCreateUserWithEmptyEmail",
        description: "Cannot create user with empty email.");

    public static readonly Error MemberAlreadyAdded = Error.Validation(
        code: "User.MemberAlreadyAdded",
        description: "Member is already added to the user.");

    public static readonly Error MemberNotFound = Error.Validation(
        code: "User.MemberNotFound",
        description: "Member is not found in the user.");

    public static readonly Error UserAlreadyExists = Error.Validation(
        code: "User.UserAlreadyExists",
        description: "User with the same email already exists.");

    public static readonly Error MaxTripsReached = Error.Validation(
        code: "User.MaxTripsReached",
        description: "User has reached the maximum number of trips.");

    public static readonly Error UserNotFound = Error.NotFound(
        code: "User.UserNotFound",
        description: "User is not found.");
}