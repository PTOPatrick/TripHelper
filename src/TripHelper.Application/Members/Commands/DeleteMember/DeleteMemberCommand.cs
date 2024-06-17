using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;

namespace TripHelper.Application.Members.Commands.DeleteMember;

[Authorize]
public record DeleteMemberCommand(int Id) : IRequest<ErrorOr<Deleted>>;