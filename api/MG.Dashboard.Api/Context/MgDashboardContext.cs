using MG.Dashboard.Api.Entities;

using Microsoft.EntityFrameworkCore;

namespace MG.Dashboard.Api.Context;

public partial class MgDashboardContext : DbContext
{
    public MgDashboardContext() { }

    public MgDashboardContext(DbContextOptions<MgDashboardContext> options)
        : base(options) { }

    public virtual DbSet<AccessKeyEntity> AccessKeys { get; set; }

    public virtual DbSet<DeviceEntity> Devices { get; set; }

    public virtual DbSet<UserEntity> Users { get; set; }

    public virtual DbSet<UserDeviceEntity> UserDevices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccessKeyEntity>(
            entity =>
            {
                entity.HasKey(e => e.Id).HasName("access_keys_pkey");

                entity.ToTable("access_keys");

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("id");
                entity.Property(e => e.Key)
                      .HasMaxLength(255)
                      .HasColumnName("key");
            });

        modelBuilder.Entity<DeviceEntity>(
            entity =>
            {
                entity.HasKey(e => e.Id).HasName("devices_pkey");

                entity.ToTable("devices");

                entity.HasIndex(e => e.Name, "name").IsUnique();

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("id");
                entity.Property(e => e.Name)
                      .HasMaxLength(255)
                      .HasColumnName("name");
                entity.Property(e => e.Type).HasColumnName("type");
            });

        modelBuilder.Entity<UserEntity>(
            entity =>
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

                entity.HasOne(d => d.AccessKey)
                      .WithMany(p => p.Users)
                      .HasForeignKey(d => d.AccessKeyId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("access_key_id");
            });

        modelBuilder.Entity<UserDeviceEntity>(
            entity =>
            {
                entity.HasKey(e => e.Id).HasName("user_devices_pkey");

                entity.ToTable("user_devices");

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("id");
                entity.Property(e => e.DeviceId).HasColumnName("device_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Device)
                      .WithMany(p => p.UserDevices)
                      .HasForeignKey(d => d.DeviceId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("device_id");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.UserDevices)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("user_id");
            });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}