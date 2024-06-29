using MediatR;
using NSubstitute;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.SubcutaneousTests.Common;
using TestCommon.TestConstants;
using TripHelper.Domain.Trips;
using TestCommon.Trips;
using FluentAssertions;
using ErrorOr;

namespace TripHelper.Application.SubcutaneousTests.Trips.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class DeleteTripTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly ITripsRepository _tripsRepository = Substitute.For<ITripsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task DeleteTrip_WhenValidData_ShouldReturnDeleted()
    {
        // Arrange
        var trip = await CreateTrip();

        _authorizationService.CanDeleteTrip(trip.Id).Returns(true);
        _tripsRepository.GetTripByIdAsync(trip.Id).Returns(trip);

        var command = TripCommandFactory.CreateDeleteTripCommand(trip.Id);
        var handler = TripCommandFactory.CreateDeleteTripCommandHandler(_tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(Result.Deleted);
    }

    [Fact]
    public async Task DeleteTrip_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        _authorizationService.CanDeleteTrip(Constants.Trip.Id).Returns(false);

        var command = TripCommandFactory.CreateDeleteTripCommand(Constants.Trip.Id);
        var handler = TripCommandFactory.CreateDeleteTripCommandHandler(_tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.Unauthorized());
    }

    [Fact]
    public async Task DeleteTrip_WhenTripNotFound_ShouldReturnTripNotFound()
    {
        // Arrange
        _authorizationService.CanDeleteTrip(Constants.Trip.Id).Returns(true);
        _tripsRepository.GetTripByIdAsync(Constants.Trip.Id).Returns(null as Trip);

        var command = TripCommandFactory.CreateDeleteTripCommand(Constants.Trip.Id);
        var handler = TripCommandFactory.CreateDeleteTripCommandHandler(_tripsRepository, _unitOfWork, _authorizationService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TripErrors.TripNotFound);
    }

    private async Task<Trip> CreateTrip()
    {
        var createTripCommand = TripCommandFactory.CreateCreateTripCommand(
            Constants.Trip.Name,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            Constants.Trip.Description,
            Constants.Trip.Location,
            Constants.Trip.ImageUrl
        );

        var result = await _mediator.Send(createTripCommand);

        result.IsError.Should().BeFalse();
        result.Value.Name.Should().Be(Constants.Trip.Name);
        result.Value.Description.Should().Be(Constants.Trip.Description);
        result.Value.Location.Should().Be(Constants.Trip.Location);
        result.Value.ImageUrl.Should().Be(Constants.Trip.ImageUrl);

        return result.Value;
    }
}