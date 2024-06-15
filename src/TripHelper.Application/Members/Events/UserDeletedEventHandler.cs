using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Users.Events;

namespace TripHelper.Application.Members.Events;

public class UserDeletedEventHandler(
    IMembersRepository membersRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<UserDeletedEvent>
{
    private readonly IMembersRepository _membersRepository = membersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
    {
        var members = await _membersRepository.GetMembersByUserIdAsync(notification.UserId);

        await _membersRepository.DeleteMembersAsync(members);
        await _unitOfWork.CommitChangesAsync();
    }
}