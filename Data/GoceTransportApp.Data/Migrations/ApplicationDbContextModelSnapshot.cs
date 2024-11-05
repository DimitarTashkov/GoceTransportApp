﻿// <auto-generated />
using System;
using GoceTransportApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GoceTransportApp.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GoceTransportApp.Data.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<Guid>("CityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("ProfilePictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.CityStreet", b =>
                {
                    b.Property<Guid>("CityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("StreetId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("CityId", "StreetId");

                    b.HasIndex("StreetId");

                    b.ToTable("CitiesStreets");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Driver", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Age")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Experience")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Drivers");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("SenderId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("FounderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Phone")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("FounderId");

                    b.HasIndex("IsDeleted");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Report", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CoursesCompleted")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("Expenses")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Revenue")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TicketsSold")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Route", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<double>("Distance")
                        .HasColumnType("float");

                    b.Property<double>("Duration")
                        .HasColumnType("float");

                    b.Property<Guid>("FromCityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FromStreetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ToCityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ToStreetId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("FromCityId");

                    b.HasIndex("FromStreetId");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("ToCityId");

                    b.HasIndex("ToStreetId");

                    b.ToTable("Routes");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Arrival")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Day")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Departure")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RouteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("RouteId");

                    b.HasIndex("VehicleId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Street", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Streets");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DeletedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("IssuedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("RouteId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("RouteId");

                    b.HasIndex("ScheduleId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<double>("FuelConsumption")
                        .HasColumnType("float");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("VehicleStatus")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.HasIndex("OrganizationId");

                    b.ToTable("Vehicles");
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

            modelBuilder.Entity("GoceTransportApp.Data.Models.ApplicationUser", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.City", "City")
                        .WithMany("CityUsers")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.CityStreet", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.City", "City")
                        .WithMany("CitiesStreets")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Street", "Street")
                        .WithMany("StreetsCities")
                        .HasForeignKey("StreetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("City");

                    b.Navigation("Street");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Driver", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.Organization", "Organization")
                        .WithMany("OrganizationDrivers")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Message", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.Organization", "Organization")
                        .WithMany("OrganizationMessages")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.ApplicationUser", "Sender")
                        .WithMany("UserMessages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Organization", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.ApplicationUser", "Founder")
                        .WithMany("UserOrganizations")
                        .HasForeignKey("FounderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Founder");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Report", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.Organization", "Organization")
                        .WithMany("OrganizationReports")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Route", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.City", "FromCity")
                        .WithMany("FromCityRoutes")
                        .HasForeignKey("FromCityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Street", "FromStreet")
                        .WithMany("FromStreetRoutes")
                        .HasForeignKey("FromStreetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.City", "ToCity")
                        .WithMany("ToCityRoutes")
                        .HasForeignKey("ToCityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Street", "ToStreet")
                        .WithMany("ToStreetRoutes")
                        .HasForeignKey("ToStreetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FromCity");

                    b.Navigation("FromStreet");

                    b.Navigation("ToCity");

                    b.Navigation("ToStreet");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Schedule", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.Organization", "Organization")
                        .WithMany("OrganizationSchedules")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Route", "Route")
                        .WithMany("RouteSchedules")
                        .HasForeignKey("RouteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Vehicle", "Vehicle")
                        .WithMany("Schedules")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");

                    b.Navigation("Route");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Ticket", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.ApplicationUser", "Customer")
                        .WithMany("UserTickets")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Organization", "Organization")
                        .WithMany("OrganizationTickets")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Route", "Route")
                        .WithMany("RouteTickets")
                        .HasForeignKey("RouteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.Schedule", "TimeTable")
                        .WithMany("ScheduleTickets")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Organization");

                    b.Navigation("Route");

                    b.Navigation("TimeTable");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Vehicle", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.ApplicationUser", null)
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.ApplicationUser", null)
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GoceTransportApp.Data.Models.ApplicationUser", null)
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("GoceTransportApp.Data.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.ApplicationUser", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Logins");

                    b.Navigation("Roles");

                    b.Navigation("UserMessages");

                    b.Navigation("UserOrganizations");

                    b.Navigation("UserTickets");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.City", b =>
                {
                    b.Navigation("CitiesStreets");

                    b.Navigation("CityUsers");

                    b.Navigation("FromCityRoutes");

                    b.Navigation("ToCityRoutes");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Organization", b =>
                {
                    b.Navigation("OrganizationDrivers");

                    b.Navigation("OrganizationMessages");

                    b.Navigation("OrganizationReports");

                    b.Navigation("OrganizationSchedules");

                    b.Navigation("OrganizationTickets");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Route", b =>
                {
                    b.Navigation("RouteSchedules");

                    b.Navigation("RouteTickets");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Schedule", b =>
                {
                    b.Navigation("ScheduleTickets");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Street", b =>
                {
                    b.Navigation("FromStreetRoutes");

                    b.Navigation("StreetsCities");

                    b.Navigation("ToStreetRoutes");
                });

            modelBuilder.Entity("GoceTransportApp.Data.Models.Vehicle", b =>
                {
                    b.Navigation("Schedules");
                });
#pragma warning restore 612, 618
        }
    }
}
