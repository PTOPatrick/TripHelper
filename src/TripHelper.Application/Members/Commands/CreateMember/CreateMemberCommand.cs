using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Members.Commands.CreateMember;

[Authorize]
public record CreateMemberCommand(string Email, int TripId, bool IsAdmin) : IRequest<ErrorOr<MemberWithEmail>>;