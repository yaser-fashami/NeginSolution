using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negin.Infra.Data.Sql.Configurations;

internal class ShippingLineCompanyConfig : IEntityTypeConfiguration<ShippingLineCompany>
{
	public void Configure(EntityTypeBuilder<ShippingLineCompany> builder)
	{
		builder.ToTable("ShippingLineCompany", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x=>x.ShippingLineName).IsRequired().HasMaxLength(50);
		builder.Property(x=>x.EconomicCode).IsRequired().IsFixedLength().HasMaxLength(20);
		builder.Property(x=>x.NationalCode).IsRequired().IsFixedLength().HasMaxLength(20);
		builder.Property(x=>x.Tel).HasMaxLength(200);
		builder.Property(x=>x.Email).HasColumnType("varchar(50)").HasMaxLength(50);
		builder.Property(x => x.Address).HasMaxLength(200);
		builder.Property(x => x.City).HasMaxLength(20);
		builder.Property(x => x.Description).HasMaxLength(1000);
		builder.Property(x => x.CreatedById).IsRequired().HasMaxLength(450);
		builder.Property(x => x.ModifiedById).HasMaxLength(450);
		builder.Property(x => x.IsDelete).IsRequired().HasDefaultValue(false);
		builder.Property(x => x.IsOwner).IsRequired();
		builder.Property(x => x.IsAgent).IsRequired();

		builder.HasIndex(c => c.ShippingLineName).IsUnique();

		#region Navigation
		builder.HasMany(c => c.Agents).WithOne(d => d.ShippingLineCompany).HasForeignKey(s=>s.ShippingLineCompanyId).OnDelete(DeleteBehavior.Cascade);
		builder.HasMany(c => c.OwnerVoyages).WithOne(d => d.Owner).HasForeignKey(s=>s.OwnerId).OnDelete(DeleteBehavior.NoAction);
		builder.HasMany(c => c.AgentVoyages).WithOne(d => d.Agent).HasForeignKey(s=>s.AgentId).OnDelete(DeleteBehavior.NoAction);

		#endregion
	}
}
