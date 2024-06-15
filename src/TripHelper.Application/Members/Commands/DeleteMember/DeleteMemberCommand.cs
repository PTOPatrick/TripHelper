using ErrorOr;
using MediatR;

namespace TripHelper.Application.Members.Commands.DeleteMember;

public record DeleteMemberCommand(int Id) : IRequest<ErrorOr<Deleted>>;