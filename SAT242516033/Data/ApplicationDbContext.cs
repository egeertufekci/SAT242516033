using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrunSiparisTakip.Models;

namespace SAT242516033.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain DbSets for Urun/Siparis models so pages can use EF directly
        public DbSet<Urun> Urunler { get; set; } = null!;
        public DbSet<Kategori> Kategoriler { get; set; } = null!;
        public DbSet<Musteri> Musteriler { get; set; } = null!;
        public DbSet<Siparis> Siparisler { get; set; } = null!;
        public DbSet<SiparisDetayi> SiparisDetaylari { get; set; } = null!;
        public DbSet<UrunKategori> UrunKategorileri { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Composite key for join table
            builder.Entity<UrunKategori>()
                .HasKey(uk => new { uk.UrunId, uk.KategoriId });
        }
    }
}
