using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Trips.Commands.CreateTrip;
using TripHelper.Application.Trips.Commands.DeleteTrip;
using TripHelper.Application.Trips.Commands.UpdateTrip;

namespace TestCommon.Trips;

public static class TripCommandFactory
{
    public static CreateTripCommand CreateCreateTripCommand(
        string name,
        DateTime? startDate,
        DateTime? endDate,
        string? description,
        string? location,
        string? imageUrl)
    {
        return new CreateTripCommand(name, startDate, endDate, description, location, imageUrl);
    }

    public static DeleteTripCommand CreateDeleteTripCommand(int tripId)
    {
        return new DeleteTripCommand(tripId);
    }

    public static UpdateTripCommand CreateUpdateTripCommand(
        int tripId,
        string name,
        DateTime? startDate,
        DateTime? endDate,
        string? description,
        string? location,
        string? imageUrl)
    {
        return new UpdateTripCommand(tripId, name, startDate, endDate, description, location, imageUrl);
    }

    public static CreateTripCommandHandler CreateCreateTripCommandHandler(
        ITripsRepository tripsRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new CreateTripCommandHandler(tripsRepository, unitOfWork, authorizationService);
    }

    public static DeleteTripCommandHandler CreateDeleteTripCommandHandler(
        ITripsRepository tripsRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new DeleteTripCommandHandler(tripsRepository, unitOfWork, authorizationService);
    }

    public static UpdateTripCommandHandler CreateUpdateTripCommandHandler(
        ITripsRepository tripsRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new UpdateTripCommandHandler(tripsRepository, unitOfWork, authorizationService);
    }
}