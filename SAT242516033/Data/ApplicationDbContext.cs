using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrunSiparisTakip.Models;

namespace SAT242516033.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Urun> Urunler { get; set; } = null!;
        public DbSet<Kategori> Kategoriler { get; set; } = null!;
        public DbSet<Musteri> Musteriler { get; set; } = null!;
        public DbSet<Siparis> Siparisler { get; set; } = null!;
        public DbSet<SiparisDetayi> SiparisDetaylari { get; set; } = null!;
        public DbSet<UrunKategori> UrunKategorileri { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Urun>().ToTable("Urunler");
            builder.Entity<Urun>().HasKey(x => x.UrunId);

            builder.Entity<UrunKategori>().ToTable("UrunKategorileri");
            builder.Entity<UrunKategori>().HasKey(uk => new { uk.UrunId, uk.KategoriId });
        }
    }
}
