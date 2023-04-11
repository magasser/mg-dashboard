using System;
using System.Collections.Generic;
using MG.Dashboard.Api.Entities;
using MG.Dashboard.Api.Entities.Types;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MG.Dashboard.Api.Context;

public partial class MgDashboardContext : DbContext
{
    public MgDashboardContext()
    {
    }

    public MgDashboardContext(DbContextOptions<MgDashboardContext> options)
        : base(options)
    {
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DeviceRole>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DeviceType>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<KeyType>();
        NpgsqlConnection.GlobalTypeMapper.MapEnum<UserRole>();
    }

    public virtual DbSet<AccessKey> AccessKeys { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDevice> UserDevices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum<DeviceRole>()
            .HasPostgresEnum<DeviceType>()
            .HasPostgresEnum<KeyType>()
            .HasPostgresEnum<UserRole>();

        modelBuilder.Entity<AccessKey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("access_keys_pkey");

            entity.ToTable("access_keys");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Type)
                  .HasColumnName("type");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("devices_pkey");

            entity.ToTable("devices");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccessKeyId).HasColumnName("access_key_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.Type)
                  .HasColumnName("type");

            entity.HasOne(d => d.AccessKey).WithMany(p => p.Devices)
                .HasForeignKey(d => d.AccessKeyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("devices_access_key_id_fkey");

            entity.HasOne(d => d.Owner).WithMany(p => p.Devices)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("devices_owner_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AccessKeyId).HasColumnName("access_key_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                  .HasColumnName("role");

            entity.HasOne(d => d.AccessKey).WithMany(p => p.Users)
                .HasForeignKey(d => d.AccessKeyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_access_key_id_fkey");
        });

        modelBuilder.Entity<UserDevice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_devices_pkey");

            entity.ToTable("user_devices");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Role)
                  .HasColumnName("role");

            entity.HasOne(d => d.Device).WithMany(p => p.UserDevices)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_devices_device_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserDevices)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_devices_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
