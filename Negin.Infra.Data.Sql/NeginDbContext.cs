using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Billing;
using Negin.Framework.Utilities;
using Negin.Infra.Data.Sql.Configurations;

namespace Negin.Infrastructure;
public class NeginDbContext : IdentityDbContext<User>
{
	public NeginDbContext(DbContextOptions options) : base(options)
	{

	}

	#region Basic
	public DbSet<Vessel> Vessels { get; set; }
	public DbSet<Country> Countries { get; set; }
	public DbSet<VesselType> VesselTypes { get; set; }

	public DbSet<ShippingLineCompany> ShippingLines { get; set; }
	public DbSet<AgentShippingLine> AgentsShippingLine { get; set; }

	public DbSet<Voyage> Voyages { get; set; }
	public DbSet<Port> Ports { get; set; }

	#endregion

	#region Billing
	public DbSet<VesselStoppage> VesselStoppages { get; set; }
	#endregion

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
		optionsBuilder.UseExceptionProcessor();
    }

    protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfiguration(new CountryConfig());
		builder.ApplyConfiguration(new UserConfig());
		builder.ApplyConfiguration(new VesselConfig());
		builder.ApplyConfiguration(new VesselTypeConfig());
		builder.ApplyConfiguration(new ShippingLineCompanyConfig());
		builder.ApplyConfiguration(new AgentShippingLineConfig());
		builder.ApplyConfiguration(new PortConfig());
		builder.ApplyConfiguration(new VoyageConfig());
		builder.ApplyConfiguration(new VesselStoppageConfig());


		var user = new User() { Id = Guid.NewGuid().ToString(), UserName="admin", PasswordHash = Util.GetHashString("123"), IsActived = true };

		builder.Entity<IdentityRole>().HasData(new IdentityRole() { Id = "1", Name = "admin", NormalizedName = "admin" },
												new IdentityRole() { Id = "2", Name = "default", NormalizedName = "default" });

		builder.Entity<User>().HasData(user);
		builder.Entity<IdentityUserRole<string>>().HasData(new { RoleId = "1", UserId = user.Id });

	}

}
