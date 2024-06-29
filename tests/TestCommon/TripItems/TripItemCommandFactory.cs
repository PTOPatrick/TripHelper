using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.TripItems.Commands.CreateTripItem;
using TripHelper.Application.TripItems.Commands.DeleteTripItem;
using TripHelper.Application.TripItems.Commands.UpdateTripItem;

namespace TestCommon.TripItems;

public static class TripItemCommandFactory
{
    public static CreateTripItemCommand CreateCreateTripItemCommand(
        string name,
        int tripId,
        decimal amount,
        int memberId)
    {
        return new CreateTripItemCommand(name, tripId, amount, memberId);
    }

    public static UpdateTripItemCommand CreateUpdateTripItemCommand(
        int tripId,
        int tripItemId,
        string name,
        decimal amount,
        int memberId)
    {
        return new UpdateTripItemCommand(tripId, tripItemId, name, amount, memberId);
    }

    public static DeleteTripItemCommand CreateDeleteTripItemCommand(int tripId, int tripItemId)
    {
        return new DeleteTripItemCommand(tripId, tripItemId);
    }

    public static CreateTripItemCommandHandler CreateCreateTripItemCommandHandler(
        ITripItemsRepository tripItemsRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new CreateTripItemCommandHandler(tripItemsRepository, unitOfWork, authorizationService);
    }

    public static UpdateTripItemCommandHandler CreateUpdateTripItemCommandHandler(
        ITripItemsRepository tripItemsRepository,
        IMembersRepository membersRepository,
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new UpdateTripItemCommandHandler(tripItemsRepository, membersRepository, usersRepository, unitOfWork, authorizationService);
    }

    public static DeleteTripItemCommandHandler CreateDeleteTripItemCommandHandler(
        ITripItemsRepository tripItemsRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new DeleteTripItemCommandHandler(tripItemsRepository, unitOfWork, authorizationService);
    }
}