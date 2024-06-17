using Microsoft.EntityFrameworkCore;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Users;
using TripHelper.Infrastructure.Common.Persistence;

namespace TripHelper.Infrastructure.Users.Persistence;

public class UsersRepository(TripHelperDbContext dbContext) : IUsersRepository
{
    private readonly TripHelperDbContext _dbContext = dbContext;
    
    public async Task AddUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
    }

    public Task DeleteUserAsync(User user)
    {
        _dbContext.Remove(user);

        return Task.CompletedTask;
    }

    public Task UpdateUserAsync(User user)
    {
        _dbContext.Update(user);

        return Task.CompletedTask;
    }

    public async Task<List<User>> GetUsersByIdsAsync(List<int> userIds)
    {
        return await _dbContext.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email.ToLower().Equals(email.ToLower()));
    }
}
