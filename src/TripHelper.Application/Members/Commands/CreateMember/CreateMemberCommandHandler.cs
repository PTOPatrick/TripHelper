using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Members.Commands.CreateMember;

public class CreateMemberCommandHandler(
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    ITripsRepository _tripsRepository,
    IUnitOfWork _unitOfWork
) : IRequestHandler<CreateMemberCommand, ErrorOr<Member>>
{
    private readonly IMembersRepository _membersRepository = _membersRepository;
    private readonly IUsersRepository _usersRepository = _usersRepository;
    private readonly ITripsRepository _tripsRepository = _tripsRepository;
    private readonly IUnitOfWork _unitOfWork = _unitOfWork;
    private User? _user;

    public async Task<ErrorOr<Member>> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateRequest(request);
        if (validationResult.IsError)
            return validationResult.Errors;

        var member = new Member(_user!.Id, request.TripId, request.IsAdmin);

        await _membersRepository.AddMemberAsync(member);
        await _unitOfWork.CommitChangesAsync();

        return member;
    }

    private async Task<ErrorOr<Success>> ValidateRequest(CreateMemberCommand request)
    {
        _user = await _usersRepository.GetUserByEmailAsync(request.Email);
        if (_user is null)
            return MemberErrors.UserNotFound;

        var trip = await _tripsRepository.GetTripByIdAsync(request.TripId);
        if (trip is null)
            return MemberErrors.TripNotFound;

        return Result.Success;
    }
}