using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using TripHelper.Application.Common.Models;

namespace TripHelper.Application.Members.Queries.GetMember;

[Authorize]
public record GetMemberQuery(int Id) : IRequest<ErrorOr<MemberWithEmail>>;