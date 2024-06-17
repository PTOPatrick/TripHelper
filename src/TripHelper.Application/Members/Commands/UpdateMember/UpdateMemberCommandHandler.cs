using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Commands.UpdateMember;

public class UpdateMemberCommandHandler(
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    IUnitOfWork _unitOfWork,
    AuthorizationService _authorizationService
) : IRequestHandler<UpdateMemberCommand, ErrorOr<MemberWithEmail>>
{
    public async Task<ErrorOr<MemberWithEmail>> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _membersRepository.GetMemberAsync(request.Id); 
        if (member is null)
            return MemberErrors.MemberNotFound;

        if (!_authorizationService.CanUpdateMember(member.TripId))
            return Error.Unauthorized();

        var user = await _usersRepository.GetUserByIdAsync(member.UserId);
        if (user is null)
            return MemberErrors.UserNotFound;

        member.Update(request.IsAdmin);
        
        await _membersRepository.UpdateMemberAsync(member);
        await _unitOfWork.CommitChangesAsync();

        return new MemberWithEmail(member.Id, member.UserId, member.TripId, member.IsAdmin, user!.Email);
    }
}