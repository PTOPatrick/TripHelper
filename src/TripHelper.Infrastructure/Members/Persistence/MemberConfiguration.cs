using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripHelper.Domain.Members;

namespace TripHelper.Infrastructure.Members.Persistence;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.UserId).IsRequired();
        builder.Property(m => m.TripId).IsRequired();
        builder.Property(m => m.IsAdmin).IsRequired();
    }
}