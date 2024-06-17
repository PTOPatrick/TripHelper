using Microsoft.EntityFrameworkCore;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;
using TripHelper.Infrastructure.Common.Persistence;

namespace TripHelper.Infrastructure.Members.Persistence
{
    public class MembersRepository(TripHelperDbContext dbContext) : IMembersRepository
    {
        private readonly TripHelperDbContext _dbContext = dbContext;

        public async Task AddMemberAsync(Member member)
        {
            await _dbContext.Members.AddAsync(member);
        }

        public async Task<List<Member>> GetMembersByUserIdAsync(int userId)
        {
            return await _dbContext.Members.Where(m => m.UserId == userId).ToListAsync();
        }

        public Task DeleteMembersAsync(List<Member> members)
        {
            _dbContext.RemoveRange(members);

            return Task.CompletedTask;
        }

        public async Task<Member?> GetMemberAsync(int id)
        {
            return await _dbContext.Members.FindAsync(id);
        }

        public Task UpdateMemberAsync(Member member)
        {
            _dbContext.Update(member);

            return Task.CompletedTask;
        }

        public Task DeleteMemberAsync(Member member)
        {
            _dbContext.Remove(member);

            return Task.CompletedTask;
        }

        public async Task<List<Member>> GetMembersByTripIdAsync(int tripId)
        {
            return await _dbContext.Members.Where(m => m.TripId == tripId).ToListAsync();
        }

        public List<Member> GetMembersByUserId(int userId)
        {
            return [.. _dbContext.Members.Where(m => m.UserId == userId)];
        }

        public async Task<Member?> GetMemberByUserIdAndTripIdAsync(int userId, int tripId)
        {
            return await _dbContext.Members.FirstOrDefaultAsync(m => m.UserId == userId && m.TripId == tripId);
        }
    }
}