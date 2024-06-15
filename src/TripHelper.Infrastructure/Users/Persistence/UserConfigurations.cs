using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripHelper.Domain.Users;

namespace TripHelper.Infrastructure.Users.Persistence;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired();
        builder.Property(u => u.Firstname).IsRequired();
        builder.Property(u => u.Lastname).IsRequired();
        builder.Property(u => u.Password).IsRequired();
        builder.Property(u => u.IsSuperAdmin).IsRequired();
        builder.Property("_memberIds").HasColumnName("MemberIds").IsRequired(false);
    }
}