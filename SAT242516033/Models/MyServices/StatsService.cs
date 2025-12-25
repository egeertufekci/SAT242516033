using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SAT242516033.Data; // ApplicationUser burada
using SAT242516033.Models.DbContexts; // MyDbModel_DbContext burada (Namespace'i kontrol et!)
using System.Threading.Tasks;

public class StatsService
{
    private readonly MyDbModel_DbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StatsService(MyDbModel_DbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<StatsViewModel> GetStatsAsync()
    {
        var stats = new StatsViewModel
        {
            // Veritabanındaki tablolardan sayıları alıyoruz
            ToplamUrun = await _context.Urunler.CountAsync(),
            ToplamMusteri = await _context.Musteriler.CountAsync(),
            ToplamSiparis = await _context.Siparisler.CountAsync(),
            ToplamKullanici = await _userManager.Users.CountAsync()
        };

        return stats;
    }
}