﻿// <auto-generated />
using System;
using FarmMonitor.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FarmMonitor.Migrations
{
    [DbContext(typeof(FarmMonitorDbContext))]
    partial class FarmMonitorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("farm-monitor")
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FarmMonitor.Domain.Entities.DeviceEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("FarmId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("DeviceEntity", "farm-monitor");
                });

            modelBuilder.Entity("FarmMonitor.Domain.Entities.TelemetryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uuid");

                    b.Property<float>("Humidity")
                        .HasColumnType("real");

                    b.Property<DateTimeOffset>("MeasurementDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("Temperature")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("TelemetryEntity", "farm-monitor");
                });

            modelBuilder.Entity("FarmMonitor.Domain.Entities.TelemetryEntity", b =>
                {
                    b.HasOne("FarmMonitor.Domain.Entities.DeviceEntity", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");
                });
#pragma warning restore 612, 618
        }
    }
}
