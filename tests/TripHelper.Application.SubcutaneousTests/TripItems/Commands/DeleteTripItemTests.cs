using MediatR;
using TestCommon.Trips;
using TripHelper.Application.SubcutaneousTests.Common;
using TripHelper.Domain.Trips;
using TestCommon.TestConstants;
using FluentAssertions;
using TripHelper.Domain.TripItems;
using TripHelper.Application.Common.Interfaces;
using NSubstitute;
using TestCommon.TripItems;
using ErrorOr;

namespace TripHelper.Application.SubcutaneousTests.TripItems.Commands;

[Collection(MediatorFactoryCollection.CollectionName)]
public class DeleteTripItemTests(MediatorFactory mediatorFactory)
{
    private readonly IMediator _mediator = mediatorFactory.CreateMediator();
    private readonly ITripItemsRepository _tripItemsRepository = Substitute.For<ITripItemsRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();

    [Fact]
    public async Task DeleteTripItem_WhenUserIsAllowedToDeleteTripItem_ShouldReturnDeleted()
    {
        // Arrange
        var tripItem = await CreateTripItem();

        _authorizationService.CanDeleteTripItem(tripItem.TripId).Returns(true);
        _tripItemsRepository.GetTripItemAsync(tripItem.Id).Returns(tripItem);

        var command = TripItemCommandFactory.CreateDeleteTripItemCommand(tripItem.TripId, tripItem.Id);

        var handler = TripItemCommandFactory.CreateDeleteTripItemCommandHandler(
            _tripItemsRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);
    }

    [Fact]
    public async Task DeleteTripItem_WhenUnauthorized_ShouldReturnUnauthorized()
    {
        // Arrange
        var tripItem = await CreateTripItem();

        _authorizationService.CanDeleteTripItem(tripItem.TripId).Returns(false);

        var command = TripItemCommandFactory.CreateDeleteTripItemCommand(tripItem.TripId, tripItem.Id);

        var handler = TripItemCommandFactory.CreateDeleteTripItemCommandHandler(
            _tripItemsRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(Error.Unauthorized());
    }

    [Fact]
    public async Task DeleteTripItem_WhenTripItemNotFound_ShouldReturnTripItemNotFound()
    {
        // Arrange
        var tripItem = await CreateTripItem();

        _authorizationService.CanDeleteTripItem(tripItem.TripId).Returns(true);
        _tripItemsRepository.GetTripItemAsync(tripItem.Id).Returns(null as TripItem);

        var command = TripItemCommandFactory.CreateDeleteTripItemCommand(tripItem.TripId, tripItem.Id);

        var handler = TripItemCommandFactory.CreateDeleteTripItemCommandHandler(
            _tripItemsRepository,
            _unitOfWork,
            _authorizationService
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().BeEquivalentTo(TripItemErrors.TripItemNotFound);
    }

    public async Task<TripItem> CreateTripItem()
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

        return result.Value;
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