using ErrorOr;
using TripHelper.Domain.Members;

namespace TripHelper.Application.Common.Interfaces;

public interface IMembersRepository
{
    Task AddMemberAsync(Member member);
    Task DeleteMemberAsync(Member member);
    Task DeleteMembersAsync(List<Member> members);
    Task<Member?> GetMemberAsync(int id);
    Task<List<Member>> GetMembersByUserIdAsync(int userId);
    Task UpdateMemberAsync(Member member);
}