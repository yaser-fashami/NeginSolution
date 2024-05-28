using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Operation;

namespace Negin.Infra.Data.Sql.Configurations;
internal class LoadingDischargeConfig : IEntityTypeConfiguration<LoadingDischarge>
{
	public void Configure(EntityTypeBuilder<LoadingDischarge> builder)
	{
		builder.ToTable(nameof(LoadingDischarge), "Operation");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Method).IsRequired().HasMaxLength(4);
		builder.Property(x => x.Tonage).IsRequired().HasColumnType("float");
		builder.Property(x => x.HasCrane).IsRequired();
		builder.Property(x => x.HasInventory).IsRequired();
	}
}
