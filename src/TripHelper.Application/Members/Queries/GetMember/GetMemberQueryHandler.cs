using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Queries.GetMember;

public class GetMemberQueryHandler(IMembersRepository membersRepository) : IRequestHandler<GetMemberQuery, ErrorOr<Member>>
{
    private readonly IMembersRepository _membersRepository = membersRepository;

    public async Task<ErrorOr<Member>> Handle(GetMemberQuery request, CancellationToken cancellationToken)
    {
        var member = await _membersRepository.GetMemberAsync(request.Id);

        return member is not null ? member : MemberErrors.MemberNotFound;
    }
}