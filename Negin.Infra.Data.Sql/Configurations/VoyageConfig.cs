using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class VoyageConfig : IEntityTypeConfiguration<Voyage>
{
	public void Configure(EntityTypeBuilder<Voyage> builder)
	{
		builder.ToTable("Voyages", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.VoyageNoIn).IsRequired().IsUnicode(false).HasMaxLength(20);
		builder.Property(x => x.VoyageNoOut).IsRequired().IsUnicode(false).HasMaxLength(20);
		builder.Property(x => x.VesselId).IsRequired();
		builder.Property(x => x.OwnerId).IsRequired();
		builder.Property(x => x.AgentId).IsRequired();

		builder.HasIndex(x => new { x.VoyageNoIn, x.VesselId}).IsUnique();

		#region Navigation
		builder.HasOne(c => c.Owner).WithMany(d => d.OwnerVoyages).HasForeignKey(s => s.OwnerId);
		builder.HasOne(c => c.Agent).WithMany(d => d.AgentVoyages).HasForeignKey(s => s.AgentId);
		builder.HasMany(c => c.VesselStoppages).WithOne(d => d.Voyage).HasForeignKey(s => s.VoyageId);
		#endregion
	}
}
