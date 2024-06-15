using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members.Events;

namespace TripHelper.Application.Users.Events;

public class MemberDeletedEventHandler(
    IUsersRepository usersRepository,
    IUnitOfWork unitOfWork
) : INotificationHandler<MemberDeletedEvent>
{
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(MemberDeletedEvent notification, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByIdAsync(notification.UserId);

        if (user is null)
            return;
        
        user.RemoveMember(notification.MemberId);
        
        await _usersRepository.UpdateUserAsync(user);
        await _unitOfWork.CommitChangesAsync();

        throw new NotImplementedException();
    }
}