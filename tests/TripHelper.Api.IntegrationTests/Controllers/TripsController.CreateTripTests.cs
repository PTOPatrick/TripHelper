using System.Dynamic;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TestCommon.TestConstants;
using TripHelper.Api.IntegrationTests.Common;
using TripHelper.Contracts.Members;
using TripHelper.Contracts.Trips;
using TripHelper.Contracts.Users;
using TripHelper.Domain.Members;
using TripHelper.Domain.Trips;
using TripHelper.Domain.Users;

namespace TripHelper.Api.IntegrationTests.Controllers;

[Collection(TripHelperApiFactoryCollection.CollectionName)]
public class CreateTripTests
{
    private readonly HttpClient _client;
    private int _tripId;
    private int _userId;
    private int _memberId;

    public CreateTripTests(TripHelperApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        apiFactory.ResetDatabase();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CreateTrip_WhenValidTrip_ShouldCreateTrip(bool isSuperAdmin)
    {
        await CreateTrip(isSuperAdmin);
    }

    [Fact]
    public async Task UpdateTrip_WhenSuperAdmin_ShouldUpdateTrip()
    {
        // Arrange
        var trip = await CreateTrip(true);

        var updateTripRequest = new UpdateTripRequest(
            Constants.Trip.UpdatedName,
            DateTime.Now,
            DateTime.Now.AddDays(50),
            Constants.Trip.UpdatedDescription,
            Constants.Trip.UpdatedLocation,
            Constants.Trip.UpdatedImageUrl);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/trips/{_tripId}", updateTripRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tripResponse = await response.Content.ReadFromJsonAsync<TripResponse>();

        tripResponse.Should().NotBeNull();
        tripResponse!.Id.Should().NotBe(null);
        tripResponse.Name.Should().Be(Constants.Trip.UpdatedName);
        tripResponse.Description.Should().Be(Constants.Trip.UpdatedDescription);
        tripResponse.Location.Should().Be(Constants.Trip.UpdatedLocation);
        tripResponse.ImageUrl.Should().Be(Constants.Trip.UpdatedImageUrl);
        tripResponse.CreatorUserId.Should().Be(Constants.User.Id);
    }

    [Fact]
    public async Task UpdateTrip_WhenAdmin_ShouldUpdateTrip()
    {
        // Arrange
        var trip = await CreateTrip(false);
        var user = await CreateUser(false);

        var updateTripRequest = new UpdateTripRequest(
            Constants.Trip.UpdatedName,
            DateTime.Now,
            DateTime.Now.AddDays(50),
            Constants.Trip.UpdatedDescription,
            Constants.Trip.UpdatedLocation,
            Constants.Trip.UpdatedImageUrl);

        FillUpFakeAuthorizationHeader(
            false,
            Constants.User.UserTripIds,
            [_tripId]);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/trips/{_tripId}", updateTripRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tripResponse = await response.Content.ReadFromJsonAsync<TripResponse>();

        tripResponse.Should().NotBeNull();
        tripResponse!.Id.Should().NotBe(null);
        tripResponse.Name.Should().Be(Constants.Trip.UpdatedName);
        tripResponse.Description.Should().Be(Constants.Trip.UpdatedDescription);
        tripResponse.Location.Should().Be(Constants.Trip.UpdatedLocation);
        tripResponse.ImageUrl.Should().Be(Constants.Trip.UpdatedImageUrl);
        tripResponse.CreatorUserId.Should().Be(Constants.User.Id);
    }

    private async Task<User> CreateUser(bool isSuperAdmin)
    {
        // Arrange
        var createUserRequest = new CreateUserRequest(
            Constants.User.Email,
            Constants.User.Password,
            Constants.User.Firstname,
            Constants.User.Lastname,
            isSuperAdmin);

        FillUpFakeAuthorizationHeader(
            true,
            Constants.User.UserTripIds,
            Constants.User.AdminTripIds);

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", createUserRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = await response.Content.ReadFromJsonAsync<UserResponse>();

        userResponse.Should().NotBeNull();
        userResponse!.Id.Should().NotBe(null);
        userResponse.Email.Should().Be(Constants.User.Email);
        userResponse.Firstname.Should().Be(Constants.User.Firstname);
        userResponse.Lastname.Should().Be(Constants.User.Lastname);
        userResponse.IsSuperAdmin.Should().Be(isSuperAdmin);

        _userId = userResponse.Id;

        return new User(
            userResponse.Email,
            userResponse.Firstname,
            userResponse.Lastname,
            "passwordHash",
            userResponse.IsSuperAdmin
        );
    }

    private async Task<Trip> CreateTrip(bool isSuperAdmin)
    {
        // Arrange
        var createTripRequest = new CreateTripRequest(
            Constants.Trip.Name,
            DateTime.Now,
            DateTime.Now.AddDays(50),
            Constants.Trip.Description,
            Constants.Trip.Location,
            Constants.Trip.ImageUrl);

        FillUpFakeAuthorizationHeader(
            isSuperAdmin,
            Constants.User.UserTripIds,
            Constants.User.AdminTripIds);

        // Act
        var response = await _client.PostAsJsonAsync("/api/trips", createTripRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tripResponse = await response.Content.ReadFromJsonAsync<TripResponse>();

        tripResponse.Should().NotBeNull();
        tripResponse!.Id.Should().NotBe(null);
        tripResponse.Name.Should().Be(Constants.Trip.Name);
        tripResponse.Description.Should().Be(Constants.Trip.Description);
        tripResponse.Location.Should().Be(Constants.Trip.Location);
        tripResponse.ImageUrl.Should().Be(Constants.Trip.ImageUrl);
        tripResponse.CreatorUserId.Should().Be(Constants.User.Id);

        _tripId = tripResponse.Id;

        return new Trip(
            tripResponse.Name,
            tripResponse.StartDate,
            tripResponse.EndDate,
            tripResponse.Description,
            tripResponse.Location,
            tripResponse.ImageUrl,
            tripResponse.CreatorUserId
        );
    }

    private async Task<Member> CreateMember(bool isAdmin)
    {
        // Arrange
        var createMemberRequest = new CreateMemberRequest(
            Constants.User.Email,
            _tripId,
            isAdmin
        );

        FillUpFakeAuthorizationHeader(
            true,
            Constants.User.UserTripIds,
            Constants.User.AdminTripIds);

        // Act
        var response = await _client.PostAsJsonAsync("/api/members", createMemberRequest);

        // Assert
        var memberResponse = await response.Content.ReadFromJsonAsync<MemberResponse>();

        memberResponse.Should().NotBeNull();
        memberResponse!.Id.Should().NotBe(null);
        memberResponse.Email.Should().Be(Constants.User.Email);
        memberResponse.TripId.Should().Be(_tripId);

        _memberId = memberResponse.Id;

        return new Member(
            memberResponse.UserId,
            memberResponse.TripId,
            memberResponse.IsAdmin
        );
    }

    private void FillUpFakeAuthorizationHeader(
        bool isSuperAdmin,
        IReadOnlyList<int> userTripIds,
        IReadOnlyList<int> adminTripIds)
    {
        dynamic data = new ExpandoObject();
        data.id = Constants.User.Id.ToString();
        data.roles = isSuperAdmin ? Constants.User.SuperAdminUserRoles : Constants.User.RegularUserRoles;
        data.permissions = Constants.User.Permissions;
        data.userMember = string.Join(",", userTripIds);
        data.adminMember = string.Join(",", adminTripIds);

        _client.SetFakeBearerToken((object)data);
    }
}