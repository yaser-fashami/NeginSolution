using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class VesselConfig : IEntityTypeConfiguration<Vessel>
{
    public void Configure(EntityTypeBuilder<Vessel> builder)
    {
        builder.ToTable("Vessels", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.VesselTypeId).IsRequired();
        builder.Property(x => x.GrossTonage).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(8);
        builder.Property(x => x.CallSign).HasMaxLength(20);
        builder.Property(x=>x.CreatedById).IsRequired().HasMaxLength(450);
		builder.Property(x => x.ModifiedById).HasMaxLength(450);
        builder.Property(x=>x.IsDelete).IsRequired().HasDefaultValue(false);

		builder.HasIndex(c => c.Name).IsUnique();


		#region Navigation
		builder.HasOne(c => c.Nationality).WithMany(d=>d.Nationalities).HasForeignKey(s => s.NationalityId);
        builder.HasOne(c => c.Flag).WithMany(d=>d.Flags).HasForeignKey(s => s.FlagId);

        #endregion  
    }
}
