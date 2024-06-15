using ErrorOr;
using TripHelper.Domain.Users;

namespace TripHelper.Application.Common.Interfaces;

public interface IUsersRepository
{
    Task AddUserAsync(User user);
    Task DeleteUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int id);
    Task UpdateUserAsync(User user);
}