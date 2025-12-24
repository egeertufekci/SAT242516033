using Microsoft.EntityFrameworkCore;
using UrunSiparisTakip.Models;

namespace SAT242516033.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; } = null!;
        public DbSet<UretimHatti> UretimHatlari { get; set; } = null!;
        public DbSet<HataTipi> HataTipleri { get; set; } = null!;
        public DbSet<HataKaydi> HataKayitlari { get; set; } = null!;
        public DbSet<LogsTable> Logs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Kullanici>().ToTable("Kullanicilar");
            builder.Entity<UretimHatti>().ToTable("UretimHatlari");
            builder.Entity<HataTipi>().ToTable("HataTipleri");
            builder.Entity<HataKaydi>().ToTable("HataKayitlari");
            builder.Entity<LogsTable>().ToTable("LogsTable");

            builder.Entity<HataKaydi>()
                .HasOne(h => h.UretimHatti)
                .WithMany(u => u.HataKayitlari)
                .HasForeignKey(h => h.UretimHattiId);

            builder.Entity<HataKaydi>()
                .HasOne(h => h.HataTipi)
                .WithMany(t => t.HataKayitlari)
                .HasForeignKey(h => h.HataTipiId);

            builder.Entity<HataKaydi>()
                .HasOne(h => h.Kullanici)
                .WithMany(k => k.HataKayitlari)
                .HasForeignKey(h => h.KullaniciId);
        }
    }
}
