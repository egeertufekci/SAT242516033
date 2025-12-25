using Microsoft.EntityFrameworkCore;
using SAT242516033.Data; // Yeni classlarýn olduðu yer

namespace SAT242516033.Models.DbContexts // veya senin namespace'in neyse
{
    public class MyDbModel_DbContext : DbContext
    {
        public MyDbModel_DbContext(DbContextOptions<MyDbModel_DbContext> options)
            : base(options)
        {
        }

        // --- ESKÝLERÝ SÝL, BUNLARI EKLE ---
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylari { get; set; }
        public DbSet<Logs_Table> Logs_Table { get; set; }
    }
}