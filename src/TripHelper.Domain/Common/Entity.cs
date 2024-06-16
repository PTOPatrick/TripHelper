namespace TripHelper.Domain.Common;

public abstract class Entity
{
    protected List<IDomainEvent> _domainEvents = [];
    public int Id { get; set; }

    protected Entity(int id) => Id = id;

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();

        _domainEvents.Clear();

        return copy;
    }

    protected Entity() { }
}