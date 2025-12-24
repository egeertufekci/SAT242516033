using Microsoft.EntityFrameworkCore;

namespace SAT242516033.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; } = null!;
        public DbSet<UretimHatti> UretimHatlari { get; set; } = null!;
        public DbSet<HataTipi> HataTipleri { get; set; } = null!;
        public DbSet<HataKaydi> HataKayitlari { get; set; } = null!;
        public DbSet<LogsTable> LogsTables { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Kullanici>().ToTable("Kullanici");
            builder.Entity<UretimHatti>().ToTable("UretimHatti");
            builder.Entity<HataTipi>().ToTable("HataTipi");
            builder.Entity<HataKaydi>().ToTable("HataKaydi");
            builder.Entity<LogsTable>().ToTable("LogsTable");
        }
    }
}
