using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace WebImageProcessor.Models;

public partial class ImageProcessorDbContext : DbContext
{
    private readonly IConfiguration _appConfig;

    public ImageProcessorDbContext()
    {
    }

    public ImageProcessorDbContext(DbContextOptions<ImageProcessorDbContext> options, IConfiguration appConfig)
        : base(options)
    {
        _appConfig = appConfig;
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<UserRequest> UserRequests { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(_appConfig["DbConnectionString:DefaultConnection"]);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Nickname);

            entity.ToTable("AppUser");

            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nickname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("surname");

            entity.HasOne(d => d.Role).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_AppUser_UserRole");
        });

        modelBuilder.Entity<UserRequest>(entity =>
        {
            entity.ToTable("UserRequest");

            entity.Property(e => e.UserRequestId).HasColumnName("userRequestId");
            entity.Property(e => e.ColorsInPhoto)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("colorsInPhoto");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nickname");
            entity.Property(e => e.ObjectsInPhoto)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("objectsInPhoto");

            entity.HasOne(d => d.NicknameNavigation).WithMany(p => p.UserRequests)
                .HasForeignKey(d => d.Nickname)
                .HasConstraintName("FK_UserRequest_AppUser");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.ToTable("UserRole");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("roleName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
