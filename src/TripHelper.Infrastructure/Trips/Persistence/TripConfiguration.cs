using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TripHelper.Infrastructure.Trips.Persistence;

public class TripConfigurations : IEntityTypeConfiguration<Domain.Trips.Trip>
{
    public void Configure(EntityTypeBuilder<Domain.Trips.Trip> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired();
        builder.Property(t => t.CreatorUserId).IsRequired();
        builder.Property(t => t.Location).IsRequired(false);
        builder.Property(t => t.Description).IsRequired(false);
        builder.Property(t => t.StartDate).IsRequired(false);
        builder.Property(t => t.EndDate).IsRequired(false);
        builder.Property(t => t.ImageUrl).IsRequired(false);
    }
}