using ErrorOr;
using MediatR;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Commands.CreateMember;

public class CreateMemberCommandHandler(

) : IRequestHandler<CreateMemberCommand, ErrorOr<Member>>
{

    public Task<ErrorOr<Member>> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}