using FluentAssertions;
using NSubstitute;
using TestCommon.TestConstants;
using TestCommon.Trips;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Trips.Commands.CreateTrip;

namespace TripHelper.Application.SubcutaneousTests.Trips.Commands;

public class CreateTripTests
{
    private readonly ITripsRepository _tripRepository = Substitute.For<ITripsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task CreateTrip_WhenValidData_ShouldReturnTrip()
    {
        // Arrange
        var command = TripCommandFactory.CreateCreateTripCommand(
            Constants.Trip.Name,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            Constants.Trip.Description,
            Constants.Trip.Location,
            Constants.Trip.ImageUrl);

        var handler = new CreateTripCommandHandler(_tripRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(Constants.Trip.Name);
        result.Value.Description.Should().Be(Constants.Trip.Description);
        result.Value.Location.Should().Be(Constants.Trip.Location);
        result.Value.ImageUrl.Should().Be(Constants.Trip.ImageUrl);
    }
}