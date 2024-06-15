using ErrorOr;
using MediatR;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Commands.UpdateMember;

public record UpdateMemberCommand(int Id, bool IsAdmin) : IRequest<ErrorOr<Member>>;