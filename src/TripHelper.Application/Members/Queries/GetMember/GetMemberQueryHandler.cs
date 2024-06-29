using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Queries.GetMember;

public class GetMemberQueryHandler(
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    IAuthorizationService _authorizationService) : IRequestHandler<GetMemberQuery, ErrorOr<MemberWithEmail>>
{
    public async Task<ErrorOr<MemberWithEmail>> Handle(GetMemberQuery request, CancellationToken cancellationToken)
    {
        var member = await _membersRepository.GetMemberAsync(request.Id);
        if (member is null)
            return MemberErrors.MemberNotFound;

        if (!_authorizationService.CanGetMember(member.TripId))
            return Error.Unauthorized();

        var user = await _usersRepository.GetUserByIdAsync(member.UserId);
        if (user is null)
            return MemberErrors.UserNotFound;

        return new MemberWithEmail(member.Id, member.UserId, member.TripId, member.IsAdmin, user.Email);
    }
}