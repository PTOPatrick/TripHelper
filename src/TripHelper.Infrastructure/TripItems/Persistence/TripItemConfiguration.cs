using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripHelper.Domain.TripItems;

namespace TripHelper.Infrastructure.TripItems.Persistence;

public class TripItemConfigurations : IEntityTypeConfiguration<TripItem>
{
    public void Configure(EntityTypeBuilder<TripItem> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.Amount).HasPrecision(18, 4).IsRequired();
        builder.Property(t => t.TripId).IsRequired();
        builder.Property(t => t.MemberId).IsRequired();
    }
}