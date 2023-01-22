using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

public class PortConfig : IEntityTypeConfiguration<Port>
{
	public void Configure(EntityTypeBuilder<Port> builder)
	{
		builder.ToTable("Ports", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.PortName).IsRequired().HasMaxLength(50);
		builder.Property(x => x.PortEnglishName).IsUnicode(false).HasMaxLength(50);
		builder.Property(x => x.PortSymbol).IsRequired().IsFixedLength().IsUnicode(false).HasMaxLength(10);

		#region Navigation
		builder.HasOne(c => c.Country).WithMany(d => d.Ports).HasForeignKey(s => s.CountryId);
		#endregion
	}
}
