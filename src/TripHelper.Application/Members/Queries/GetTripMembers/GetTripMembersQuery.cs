using ErrorOr;
using MediatR;
using TripHelper.Application.Common.Authorization;
using TripHelper.Application.Common.Models;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Queries.GetTripMembers;

[Authorize]
public record GetTripMembersQuery(int TripId) : IRequest<ErrorOr<List<MemberWithEmail>>>;