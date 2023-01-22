using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities;

namespace Negin.Infra.Data.Sql.Configurations;

internal class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.UserName).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(4000);
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(256);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.IsActived).IsRequired();

        #region Navigation
        builder.HasMany(u => u.Roles).WithMany("Users")
            .UsingEntity<IdentityUserRole<string>>(
                userRole => userRole.HasOne<IdentityRole>()
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired(),
                userRole => userRole.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
            );

        #endregion
    }
}
