using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Commands.UpdateMember;

public class UpdateMemberCommandHandler(
    IMembersRepository membersRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateMemberCommand, ErrorOr<Member>>
{
    private readonly IMembersRepository _membersRepository = membersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<ErrorOr<Member>> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _membersRepository.GetMemberAsync(request.Id); 
        if (member is null)
            return MemberErrors.MemberNotFound;
        
        member.Update(request.IsAdmin);
        
        await _membersRepository.UpdateMemberAsync(member);
        await _unitOfWork.CommitChangesAsync();

        return member;
    }
}