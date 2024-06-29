using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Commands.DeleteMember;

public class DeleteMemberCommandHandler(
    IMembersRepository _membersRepository,
    IUnitOfWork _unitOfWork,
    IAuthorizationService _authorizationService
) : IRequestHandler<DeleteMemberCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _membersRepository.GetMemberAsync(request.Id);
        if (member is null)
            return MemberErrors.MemberNotFound;

        if (!_authorizationService.CanDeleteMember(member.TripId))
            return Error.Unauthorized();

        member.MemberDeleted();

        await _membersRepository.DeleteMemberAsync(member);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}