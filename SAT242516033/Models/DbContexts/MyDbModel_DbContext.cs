using Microsoft.EntityFrameworkCore;
using SAT242516033.Data; // Yeni classlarin oldugu yer

namespace SAT242516033.Models.DbContexts // veya senin namespace'in neyse
{
    public class MyDbModel_DbContext : DbContext
    {
        public MyDbModel_DbContext(DbContextOptions<MyDbModel_DbContext> options)
            : base(options)
        {
        }

        // --- ESKILERI SIL, BUNLARI EKLE ---
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<Urun> Urunler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kategori>(entity =>
            {
                entity.ToTable("Kategori");
                entity.HasKey(e => e.KategoriId);
                entity.Property(e => e.KategoriId)
                    .HasColumnName("KategoriId")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.KategoriAdi)
                    .HasColumnName("KategoriAdi")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.ToTable("Kullanici");
                entity.HasKey(e => e.KullaniciId);
                entity.Property(e => e.KullaniciId)
                    .HasColumnName("KullaniciId")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.AdSoyad)
                    .HasColumnName("AdSoyad")
                    .HasMaxLength(100);
                entity.Property(e => e.KullaniciAdi)
                    .HasColumnName("KullaniciAdi")
                    .HasMaxLength(50);
                entity.Property(e => e.SifreHash)
                    .HasColumnName("SifreHash")
                    .HasMaxLength(250);
                entity.Property(e => e.RolId)
                    .HasColumnName("RolId");
                entity.Property(e => e.KayitTarihi)
                    .HasColumnName("KayitTarihi")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
               
            });

            modelBuilder.Entity<Musteri>(entity =>
            {
                entity.ToTable("Musteri");
                entity.HasKey(e => e.MusteriId);
                entity.Property(e => e.MusteriId)
                    .HasColumnName("MusteriId")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.Ad)
                    .HasColumnName("Ad")
                    .HasMaxLength(50);
                entity.Property(e => e.Soyad)
                    .HasColumnName("Soyad")
                    .HasMaxLength(50);
                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(100);
                entity.Property(e => e.Telefon)
                    .HasColumnName("Telefon")
                    .HasMaxLength(20);
                entity.Property(e => e.Adres)
                    .HasColumnName("Adres")
                    .HasMaxLength(250);
                entity.Ignore(e => e.AdSoyad);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Rol");
                entity.HasKey(e => e.RolId);
                entity.Property(e => e.RolId)
                    .HasColumnName("RolId")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.RolAdi)
                    .HasColumnName("RolAdi")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Siparis>(entity =>
            {
                entity.ToTable("Siparis");
                entity.HasKey(e => e.SiparisId);
                entity.Property(e => e.SiparisId)
                    .HasColumnName("SiparisId")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.MusteriId)
                    .HasColumnName("MusteriId");
                entity.Property(e => e.SiparisTarihi)
                    .HasColumnName("SiparisTarihi")
                    .HasColumnType("datetime");
                entity.Property(e => e.Durum)
                    .HasColumnName("Durum")
                    .HasMaxLength(50);
                entity.Property(e => e.UserId)
                    .HasColumnName("UserId")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Urun>(entity =>
            {
                entity.ToTable("Urun");
                entity.HasKey(e => e.UrunId);
                entity.Property(e => e.UrunId)
                    .HasColumnName("UrunId")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.UrunAdi)
                    .HasColumnName("UrunAdi")
                    .HasMaxLength(100);
                entity.Property(e => e.KategoriId)
                    .HasColumnName("KategoriId");
                entity.Property(e => e.BirimFiyat)
                    .HasColumnName("BirimFiyat")
                    .HasColumnType("decimal(18, 2)");
                entity.Property(e => e.StokAdet)
                    .HasColumnName("StokAdet");
                entity.Property(e => e.AktifMi)
                    .HasColumnName("AktifMi")
                    .HasDefaultValueSql("((1))");
                entity.Ignore(e => e.KategoriAdi);
            });
        }
    }
}
