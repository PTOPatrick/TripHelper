using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Commands.DeleteMember;

public class DeleteMemberCommandHandler(
    IMembersRepository memberRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteMemberCommand, ErrorOr<Deleted>>
{
    private readonly IMembersRepository _memberRepository = memberRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Deleted>> Handle(DeleteMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetMemberAsync(request.Id);
        if (member is null)
            return MemberErrors.MemberNotFound;

        member.MemberDeleted();
        
        await _memberRepository.DeleteMemberAsync(member);
        await _unitOfWork.CommitChangesAsync();

        return Result.Deleted;
    }
}