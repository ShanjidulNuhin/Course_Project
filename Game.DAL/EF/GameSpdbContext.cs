using System;
using System.Collections.Generic;
using Game.DAL.EF.Tables;
using Microsoft.EntityFrameworkCore;

namespace Game.DAL.EF;

public partial class GameSpdbContext : DbContext
{
    public GameSpdbContext()
    {
    }

    public GameSpdbContext(DbContextOptions<GameSpdbContext> options)
        : base(options)
    {
    }

    // নেমস্পেস কনফ্লিক্ট এড়াতে ফুল পাথ ব্যবহার করা হয়েছে
    public virtual DbSet<Game.DAL.EF.Tables.Game> Games { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Library> Libraries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-J2IJJ4H;Database=GameSPDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Game Entity Configuration
        modelBuilder.Entity<Game.DAL.EF.Tables.Game>(entity =>
        {
            entity.ToTable("Game");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        // 2. Order Entity Configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Orders_Users");
        });

        // 3. User Entity Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        // 4. Library Entity Configuration (নতুন যোগ করা হয়েছে)
        modelBuilder.Entity<Library>(entity =>
        {
            entity.ToTable("Library");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Cart_Type).HasMaxLength(50); // Wishlist নাকি Purchased তা রাখার জন্য

            // User টেবিলের সাথে One-to-Many রিলেশন স্থাপন
            entity.HasOne(d => d.User)
                .WithMany(p => p.Libraries)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Library_Users");

            // Game টেবিলের সাথে One-to-Many রিলেশন স্থাপন
            entity.HasOne(d => d.Game)
                .WithMany(p => p.Libraries)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_Library_Game");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}