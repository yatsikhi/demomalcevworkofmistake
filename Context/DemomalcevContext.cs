using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using demomalcevworkofmistake.Models;

namespace demomalcevworkofmistake.Context;

public partial class DemomalcevContext : DbContext
{
    public DemomalcevContext()
    {
    }

    public DemomalcevContext(DbContextOptions<DemomalcevContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amount> Amounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Creator> Creators { get; set; }

    public virtual DbSet<Deliver> Delivers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=demomalcev;Username=postgres;Password=0000");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("emount_pkey");

            entity.ToTable("amount");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('emount_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasMaxLength(200)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Product).WithMany(p => p.Amounts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_amount_product");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Creator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("creator_pkey");

            entity.ToTable("creator");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Deliver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("deliver_pkey");

            entity.ToTable("deliver");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tovar_pkey");

            entity.ToTable("product");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('tovar_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.About).HasColumnName("about");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Creator).HasColumnName("creator");
            entity.Property(e => e.Deliver).HasColumnName("deliver");
            entity.Property(e => e.Discount)
                .HasMaxLength(200)
                .HasColumnName("discount");
            entity.Property(e => e.Image)
                .HasMaxLength(200)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasMaxLength(200)
                .HasColumnName("price");
            entity.Property(e => e.Units).HasColumnName("units");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Category)
                .HasConstraintName("tovar_category_fkey");

            entity.HasOne(d => d.CreatorNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Creator)
                .HasConstraintName("tovar_creator_fkey");

            entity.HasOne(d => d.DeliverNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Deliver)
                .HasConstraintName("tovar_deliver_fkey");

            entity.HasOne(d => d.UnitsNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Units)
                .HasConstraintName("tovar_units_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sale_pkey");

            entity.ToTable("sale");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Discount)
                .HasPrecision(5, 2)
                .HasColumnName("discount");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_sale_product");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unit_pkey");

            entity.ToTable("unit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login)
                .HasMaxLength(200)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .HasConstraintName("fk_users_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
