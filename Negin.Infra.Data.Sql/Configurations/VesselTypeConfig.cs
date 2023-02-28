using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class VesselTypeConfig : IEntityTypeConfiguration<VesselType>
{
    public void Configure(EntityTypeBuilder<VesselType> builder)
    {
        builder.ToTable("VesselTypes", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Description).HasMaxLength(4000);

        #region Navigation
        builder.HasMany(c=>c.Vessels).WithOne(d=>d.Type).HasForeignKey(s=>s.VesselTypeId);
        builder.HasMany(c => c.VesselStoppageTariffDetails).WithOne(d => d.VesselType).HasForeignKey("VesselTypeId");
        #endregion
    }
}
