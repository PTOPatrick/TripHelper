using FluentAssertions;
using TestCommon.Trips;

namespace TripHelper.Domain.UnitTests.Trips;

public class TripTests
{

    [Fact]
    public void Update_UpdateTrip_ShouldPass()
    {
        // Arrange
        var trip = TripFactory.CreateTrip("Trip 1", null, null, "Description 1", "Location 1", "ImageUrl 1", 1);

        // Act
        trip.Update("Trip 2", null, null, "Description 2", "Location 2", "ImageUrl 2");

        // Assert
        trip.Name.Should().Be("Trip 2");
        trip.Description.Should().Be("Description 2");
        trip.Location.Should().Be("Location 2");
        trip.ImageUrl.Should().Be("ImageUrl 2");
    }
}