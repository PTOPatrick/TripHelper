using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Models;
using TripHelper.Application.Common.Services.Authorization;
using TripHelper.Domain.Members;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Members.Commands.CreateMember;

public class CreateMemberCommandHandler(
    IMembersRepository _membersRepository,
    IUsersRepository _usersRepository,
    ITripsRepository _tripsRepository,
    IUnitOfWork _unitOfWork,
    AuthorizationService _authorizationService
) : IRequestHandler<CreateMemberCommand, ErrorOr<MemberWithEmail>>
{
    private User? _user;

    public async Task<ErrorOr<MemberWithEmail>> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        if (!_authorizationService.CanCreateMember(request.TripId))
            return Error.Unauthorized();
            
        var validationResult = await ValidateRequest(request);
        if (validationResult.IsError)
            return validationResult.Errors;

        var member = new Member(_user!.Id, request.TripId, request.IsAdmin);

        await _membersRepository.AddMemberAsync(member);
        await _unitOfWork.CommitChangesAsync();

        return new MemberWithEmail(member.Id, member.UserId, member.TripId, member.IsAdmin, _user.Email);
    }

    private async Task<ErrorOr<Success>> ValidateRequest(CreateMemberCommand request)
    {
        _user = await _usersRepository.GetUserByEmailAsync(request.Email);
        if (_user is null)
            return MemberErrors.UserNotFound;
        
        var memberCount = await _membersRepository.GetMemberCountByUserIdAsync(_user.Id);
        if (_user.HasReachedMaxMembers(memberCount))
            return MemberErrors.UserReachedMaxMembers;

        var trip = await _tripsRepository.GetTripByIdAsync(request.TripId);
        if (trip is null)
            return MemberErrors.TripNotFound;

        if (await _membersRepository.GetMemberByUserIdAndTripIdAsync(_user.Id, request.TripId) is not null)
            return MemberErrors.MemberAlreadyExists;

        return Result.Success;
    }
}