using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Members;
using TripHelper.Domain.Users;

namespace TripHelper.Infrastructure.Common.Persistence;

public class TripHelperDbContext(DbContextOptions<TripHelperDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Domain.Trips.Trip> Trips { get; set; } = null!;

    public async Task CommitChangesAsync() => await SaveChangesAsync();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}