using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Members.Commands.UpdateMember;

[Authorize]
public record UpdateMemberCommand(int Id, bool IsAdmin) : IRequest<ErrorOr<MemberWithEmail>>;