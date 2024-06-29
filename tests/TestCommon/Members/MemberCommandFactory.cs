using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Members.Commands.CreateMember;
using TripHelper.Application.Members.Commands.DeleteMember;
using TripHelper.Application.Members.Commands.UpdateMember;

namespace TestCommon.Members;

public static class MemberCommandFactory
{
    public static CreateMemberCommand CreateCreateMemberCommand(string email, int tripId, bool isAdmin)
    {
        return new CreateMemberCommand(email, tripId, isAdmin);
    }

    public static DeleteMemberCommand CreateDeleteMemberCommand(int memberId)
    {
        return new DeleteMemberCommand(memberId);
    }

    public static UpdateMemberCommand CreateUpdateMemberCommand(int memberId, bool isAdmin)
    {
        return new UpdateMemberCommand(memberId, isAdmin);
    }

    public static CreateMemberCommandHandler CreateCreateMemberCommandHandler(
        IMembersRepository memberRepository,
        IUsersRepository usersRepository,
        ITripsRepository tripsRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new CreateMemberCommandHandler(memberRepository, usersRepository, tripsRepository, unitOfWork, authorizationService);
    }

    public static DeleteMemberCommandHandler CreateDeleteMemberCommandHandler(
        IMembersRepository memberRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new DeleteMemberCommandHandler(memberRepository, unitOfWork, authorizationService);
    }

    public static UpdateMemberCommandHandler CreateUpdateMemberCommandHandler(
        IMembersRepository memberRepository,
        IUsersRepository usersRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService)
    {
        return new UpdateMemberCommandHandler(memberRepository, usersRepository, unitOfWork, authorizationService);
    }
}