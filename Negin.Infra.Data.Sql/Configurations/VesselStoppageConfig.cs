using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Aggregates.Operation;

namespace Negin.Infra.Data.Sql.Configurations;

internal class VesselStoppageConfig : IEntityTypeConfiguration<VesselStoppage>
{
	public void Configure(EntityTypeBuilder<VesselStoppage> builder)
	{
		builder.ToTable("VesselStoppage", "Operation");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.ETA).HasColumnType("smalldatetime");
		builder.Property(x => x.ATA).HasColumnType("smalldatetime");
		builder.Property(x => x.ETD).HasColumnType("smalldatetime");
		builder.Property(x => x.ATD).HasColumnType("smalldatetime");

		#region Navigation
		#endregion
	}
}
