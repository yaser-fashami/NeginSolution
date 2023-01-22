using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Billing;

namespace Negin.Infra.Data.Sql.Configurations;

internal class VesselStoppageStatusConfig : IEntityTypeConfiguration<VesselStoppageStatus>
{
	public void Configure(EntityTypeBuilder<VesselStoppageStatus> builder)
	{
		builder.ToTable("VesselStoppageStatus", "Billing");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Value).IsRequired().IsFixedLength().HasMaxLength(20);
		builder.Property(x => x.Description).HasMaxLength(200);

		#region Navigation
		#endregion
	}
}
