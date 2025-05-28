using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hüseyin.Models;

public partial class VeritabaniContext : DbContext
{
    public VeritabaniContext()
    {
    }

    public VeritabaniContext(DbContextOptions<VeritabaniContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dersler> Derslers { get; set; }

    public virtual DbSet<Duyurular> Duyurulars { get; set; }

    public virtual DbSet<OgrenciDersleri> OgrenciDersleris { get; set; }

    public virtual DbSet<Ogrenciler> Ogrencilers { get; set; }

    public virtual DbSet<Ogretmenler> Ogretmenlers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=DBlise;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dersler>(entity =>
        {
            entity.HasKey(e => e.DersId).HasName("PK__Dersler__B069CF548A635BC2");

            entity.HasOne(d => d.Ogretmen).WithMany(p => p.Derslers).HasConstraintName("FK__Dersler__ogretme__3B75D760");
        });

        modelBuilder.Entity<Duyurular>(entity =>
        {
            entity.HasKey(e => e.DuyuruId).HasName("PK__Duyurula__98E5E028E285C33B");

            entity.Property(e => e.EklenmeTarihi).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Ogretmen).WithMany(p => p.Duyurulars)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Duyurular_Ogretmen");
        });

        modelBuilder.Entity<OgrenciDersleri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OgrenciD__3213E83F4A76CA79");

            entity.HasOne(d => d.Ders).WithMany(p => p.OgrenciDersleris).HasConstraintName("FK__OgrenciDe__dersI__3F466844");

            entity.HasOne(d => d.Ogrenci).WithMany(p => p.OgrenciDersleris).HasConstraintName("FK__OgrenciDe__ogren__3E52440B");
        });

        modelBuilder.Entity<Ogrenciler>(entity =>
        {
            entity.HasKey(e => e.OgrenciId).HasName("PK__Ogrencil__91C9520CAA3F4462");
        });

        modelBuilder.Entity<Ogretmenler>(entity =>
        {
            entity.HasKey(e => e.OgretmenId).HasName("PK__Ogretmen__06E62368949B1DC2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
