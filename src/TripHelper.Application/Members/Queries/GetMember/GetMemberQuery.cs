using ErrorOr;
using MediatR;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Members.Queries.GetMember;

public record GetMemberQuery(int Id) : IRequest<ErrorOr<Member>>;