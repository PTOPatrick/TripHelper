namespace TripHelper.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}