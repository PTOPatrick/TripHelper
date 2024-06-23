using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Common;
using TripHelper.Domain.Members;
using TripHelper.Domain.TripItems;
using TripHelper.Domain.Trips;
using TripHelper.Domain.Users;

namespace TripHelper.Infrastructure.Common.Persistence;

public class TripHelperDbContext(
    DbContextOptions options,
    IHttpContextAccessor _httpContextAccessor,
    IPublisher _publisher) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<Trip> Trips { get; set; } = null!;
    public DbSet<TripItem> TripItems { get; set; } = null!;

    public async Task CommitChangesAsync()
    {
        // get hold of all the domain events
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(x => x)
            .ToList();

        // store them in the http context for later if user is waiting online
        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEvents(_publisher, domainEvents);
        }

        await SaveChangesAsync();
    }

    private static async Task PublishDomainEvents(IPublisher _publisher, List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        // fetch queue from http context or create a new queue if it doesn't exist
        var domainEventsQueue = _httpContextAccessor.HttpContext!.Items
            .TryGetValue("DomainEventsQueue", out var value) && value is Queue<IDomainEvent> existingDomainEvents
                ? existingDomainEvents
                : new Queue<IDomainEvent>();

        // add the domain events to the end of the queue
        domainEvents.ForEach(domainEventsQueue.Enqueue);

        // store the queue in the http context
        _httpContextAccessor.HttpContext!.Items["DomainEventsQueue"] = domainEventsQueue;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}