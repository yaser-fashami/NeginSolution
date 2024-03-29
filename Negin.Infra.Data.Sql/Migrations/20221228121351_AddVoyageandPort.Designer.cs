﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Negin.Infrastructure;

#nullable disable

namespace Negin.Infra.Data.Sql.Migrations
{
    [DbContext(typeof(NeginDbContext))]
    [Migration("20221228121351_AddVoyageandPort")]
    partial class AddVoyageandPort
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "1",
                            Name = "admin",
                            NormalizedName = "admin"
                        },
                        new
                        {
                            Id = "2",
                            Name = "default",
                            NormalizedName = "default"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "c114fe61-efda-45d3-b5c7-213aa48a80b2",
                            RoleId = "1"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.AgentShippingLine", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("AgentShippingLineCompanyId")
                        .HasColumnType("bigint");

                    b.Property<long>("ShippingLineCompanyId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AgentShippingLineCompanyId");

                    b.HasIndex("ShippingLineCompanyId");

                    b.ToTable("AgentShippingLine", "Basic");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FlagName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.HasKey("Id");

                    b.ToTable("Countries", "Basic");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Port", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("PortEnglishName")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PortName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PortSymbol")
                        .IsRequired()
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("char(10)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Ports", "Basic");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.ShippingLineCompany", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("City")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("EconomicCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nchar(20)")
                        .IsFixedLength();

                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("IsAgent")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDelete")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("IsOwner")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedById")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NationalCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nchar(20)")
                        .IsFixedLength();

                    b.Property<string>("ShippingLineName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Tel")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("ShippingLineName")
                        .IsUnique();

                    b.ToTable("ShippingLineCompany", "Basic");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Vessel", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<byte?>("ActiveCraneQty")
                        .HasColumnType("tinyint");

                    b.Property<byte?>("BaysQuantity")
                        .HasColumnType("tinyint");

                    b.Property<string>("CallSign")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Color")
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("FlagId")
                        .HasColumnType("int");

                    b.Property<int?>("GrossTonage")
                        .HasColumnType("int");

                    b.Property<bool>("IsDelete")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("ModifiedById")
                        .HasMaxLength(450)
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("NationalityId")
                        .HasColumnType("int");

                    b.Property<int?>("VesselLength")
                        .HasColumnType("int");

                    b.Property<byte>("VesselTypeId")
                        .HasColumnType("tinyint");

                    b.Property<int?>("VesselWidth")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("FlagId");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("NationalityId");

                    b.HasIndex("VesselTypeId");

                    b.ToTable("Vessels", "Basic");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.VesselType", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnType("tinyint");

                    b.Property<string>("Description")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("VesselTypes", "Basic");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Voyage", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<DateTime?>("ATA")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime?>("ATD")
                        .HasColumnType("smalldatetime");

                    b.Property<long>("AgentId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("ETA")
                        .HasColumnType("smalldatetime");

                    b.Property<DateTime?>("ETD")
                        .HasColumnType("smalldatetime");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<long?>("NextPortId")
                        .HasColumnType("bigint");

                    b.Property<long?>("OriginPortId")
                        .HasColumnType("bigint");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<long?>("PreviousPortId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("VesselId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("VoyageNoIn")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("VoyageNoOut")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("AgentId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("NextPortId");

                    b.HasIndex("OriginPortId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("PreviousPortId");

                    b.HasIndex("VesselId");

                    b.ToTable("Voyages", "Basic");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsActived")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLogInDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "c114fe61-efda-45d3-b5c7-213aa48a80b2",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "e69c9186-611f-4748-91ba-5f7629142b3c",
                            CreateDate = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            EmailConfirmed = false,
                            IsActived = true,
                            LockoutEnabled = false,
                            PasswordHash = "A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "48cd4ac8-38ab-485d-8802-00c2508045bf",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Negin.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.AgentShippingLine", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.Basic.ShippingLineCompany", "AgentShippingLineCompany")
                        .WithMany()
                        .HasForeignKey("AgentShippingLineCompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Negin.Core.Domain.Entities.Basic.ShippingLineCompany", "ShippingLineCompany")
                        .WithMany("Agents")
                        .HasForeignKey("ShippingLineCompanyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AgentShippingLineCompany");

                    b.Navigation("ShippingLineCompany");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Port", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.Basic.Country", "Country")
                        .WithMany("Ports")
                        .HasForeignKey("CountryId");

                    b.Navigation("Country");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.ShippingLineCompany", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.User", "CreatedBy")
                        .WithMany("ShippingLineCreated")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Negin.Core.Domain.Entities.User", "ModifiedBy")
                        .WithMany("ShippingLineModified")
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("CreatedBy");

                    b.Navigation("ModifiedBy");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Vessel", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.User", "CreatedBy")
                        .WithMany("VesselCreate")
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Negin.Core.Domain.Entities.Basic.Country", "Flag")
                        .WithMany("Flags")
                        .HasForeignKey("FlagId");

                    b.HasOne("Negin.Core.Domain.Entities.User", "ModifiedBy")
                        .WithMany("VesselModified")
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Negin.Core.Domain.Entities.Basic.Country", "Nationality")
                        .WithMany("Nationalities")
                        .HasForeignKey("NationalityId");

                    b.HasOne("Negin.Core.Domain.Entities.Basic.VesselType", "Type")
                        .WithMany("Vessels")
                        .HasForeignKey("VesselTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("Flag");

                    b.Navigation("ModifiedBy");

                    b.Navigation("Nationality");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Voyage", b =>
                {
                    b.HasOne("Negin.Core.Domain.Entities.Basic.ShippingLineCompany", "Agent")
                        .WithMany("AgentVoyages")
                        .HasForeignKey("AgentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Negin.Core.Domain.Entities.User", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Negin.Core.Domain.Entities.User", "ModifiedBy")
                        .WithMany()
                        .HasForeignKey("ModifiedById");

                    b.HasOne("Negin.Core.Domain.Entities.Basic.Port", "NextPort")
                        .WithMany()
                        .HasForeignKey("NextPortId");

                    b.HasOne("Negin.Core.Domain.Entities.Basic.Port", "OriginPort")
                        .WithMany()
                        .HasForeignKey("OriginPortId");

                    b.HasOne("Negin.Core.Domain.Entities.Basic.ShippingLineCompany", "Owner")
                        .WithMany("OwnerVoyages")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Negin.Core.Domain.Entities.Basic.Port", "PreviousPort")
                        .WithMany()
                        .HasForeignKey("PreviousPortId");

                    b.HasOne("Negin.Core.Domain.Entities.Basic.Vessel", "Vessel")
                        .WithMany()
                        .HasForeignKey("VesselId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agent");

                    b.Navigation("CreatedBy");

                    b.Navigation("ModifiedBy");

                    b.Navigation("NextPort");

                    b.Navigation("OriginPort");

                    b.Navigation("Owner");

                    b.Navigation("PreviousPort");

                    b.Navigation("Vessel");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.Country", b =>
                {
                    b.Navigation("Flags");

                    b.Navigation("Nationalities");

                    b.Navigation("Ports");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.ShippingLineCompany", b =>
                {
                    b.Navigation("AgentVoyages");

                    b.Navigation("Agents");

                    b.Navigation("OwnerVoyages");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.Basic.VesselType", b =>
                {
                    b.Navigation("Vessels");
                });

            modelBuilder.Entity("Negin.Core.Domain.Entities.User", b =>
                {
                    b.Navigation("ShippingLineCreated");

                    b.Navigation("ShippingLineModified");

                    b.Navigation("VesselCreate");

                    b.Navigation("VesselModified");
                });
#pragma warning restore 612, 618
        }
    }
}
