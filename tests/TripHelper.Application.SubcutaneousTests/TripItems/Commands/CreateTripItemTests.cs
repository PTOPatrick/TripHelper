using NSubstitute;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Trips;
using TestCommon.TestConstants;
using TestCommon.Trips;
using TripHelper.Application.SubcutaneousTests.Common;
using MediatR;
using FluentAssertions;
using TestCommon.TripItems;
using ErrorOr;
using TripHelper.Domain.TripItems;

namespace TripHelper.Application.SubcutaneousTests.TripItems.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class CreateTripItemTest(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly ITripItemsRepository _tripItemsRepository = Substitute.For<ITripItemsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task CreateTripItem_WhenUserIsAllowedToCreateTripItem_ShouldReturnTripItem()
    {
        // Arrange
        var trip = await CreateTrip();

        _authorizationService.CanCreateTripItem(trip.Id).Returns(true);

        var command = TripItemCommandFactory.CreateCreateTripItemCommand(
            Constants.TripItem.Name,
            trip.Id,
            Constants.TripItem.Amount,
            Constants.TripItem.MemberId
        );

        var handler = TripItemCommandFactory.CreateCreateTripItemCommandHandler(
            _tripItemsRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Amount.Should().Be(Constants.TripItem.Amount);
        result.Value.Name.Should().Be(Constants.TripItem.Name);
        result.Value.MemberId.Should().Be(Constants.TripItem.MemberId);
    }

    [Fact]
    public async Task CreateTripItem_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var trip = await CreateTrip();

        _authorizationService.CanCreateTripItem(trip.Id).Returns(false);

        var command = TripItemCommandFactory.CreateCreateTripItemCommand(
            Constants.TripItem.Name,
            trip.Id,
            Constants.TripItem.Amount,
            Constants.TripItem.MemberId
        );

        var handler = TripItemCommandFactory.CreateCreateTripItemCommandHandler(
            _tripItemsRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Error.Unauthorized());
    }

    [Fact]
    public async Task CreateTripItem_WhenAmountIsNegative_ShouldReturnAmountMustBePositive()
    {
        // Arrange
        var trip = await CreateTrip();

        _authorizationService.CanCreateTripItem(trip.Id).Returns(true);

        var command = TripItemCommandFactory.CreateCreateTripItemCommand(
            Constants.TripItem.Name,
            trip.Id,
            Constants.TripItem.NegativeAmount,
            Constants.TripItem.MemberId
        );

        var handler = TripItemCommandFactory.CreateCreateTripItemCommandHandler(
            _tripItemsRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(TripItemErrors.AmountMustBePositive);
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