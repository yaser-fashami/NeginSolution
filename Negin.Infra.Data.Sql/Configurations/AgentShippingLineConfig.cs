
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class AgentShippingLineConfig : IEntityTypeConfiguration<AgentShippingLine>
{
	public void Configure(EntityTypeBuilder<AgentShippingLine> builder)
	{
		builder.ToTable("AgentShippingLine", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x=>x.ShippingLineCompanyId).IsRequired();
		builder.Property(x=>x.AgentShippingLineCompanyId).IsRequired();

		#region Navigation
		builder.HasOne(c => c.ShippingLineCompany).WithMany(d => d.Agents).HasForeignKey(s => s.ShippingLineCompanyId).OnDelete(DeleteBehavior.NoAction);

		#endregion
	}
}
