using ErrorOr;
using MediatR;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Commands.CreateMember;

public record CreateMemberCommand(string Email, int TripId, bool IsAdmin) : IRequest<ErrorOr<Member>>;