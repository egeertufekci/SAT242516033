using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlazorApp_C.Data;

namespace SAT242516033.Data
{
    // ApplicationRole eklendi, key tipi string
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // (Varsa) HasNoKey tanımların burada kalabilir
            // builder.Entity<Personel>().HasNoKey();
            // ...
        }
    }
}