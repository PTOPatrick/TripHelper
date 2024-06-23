using System.Collections.Generic;

namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class User
    {
        public const int Id = 1;
        public const int DifferentUserId = 2;
        public const int DifferentTripId = 7;
        public static readonly IReadOnlyList<string> Permissions = [];
        public static readonly IReadOnlyList<string> SuperAdminUserRoles = ["Super Admin"];
        public static readonly IReadOnlyList<string> RegularUserRoles = [];
        public static readonly IReadOnlyList<int> UserTripIds = [1, 2, 3];
        public static readonly IReadOnlyList<int> AdminTripIds = [4, 5, 6];
        public const string Email = "test.user@example.com";
        public const string Firstname = "test";
        public const string Lastname = "user";
        public const string Password = "thiIS!!@1918Aa";
        public const int MoreThanUserMaxTripCount = 11;
        public const int MoreThanAdminMaxTripCount = 101;
        public const int LesshanUserMaxTripCount = 5;
        public const int LessThanAdminMaxTripCount = 50;
    }
};