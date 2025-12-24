using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAT242516033.Data;
using UrunSiparisTakip.Models;

namespace SAT242516033.Services;

public class UrunService(ApplicationDbContext context, ILogService logService)
{
    public async Task<List<Urun>> GetAllAsync(bool includeCategories = false)
    {
        var query = context.Urunler.AsQueryable();
        if (includeCategories)
        {
            query = query.Include(u => u.UrunKategorileri)
                         .ThenInclude(uk => uk.Kategori);
        }

        return await query.AsNoTracking().OrderBy(u => u.UrunId).ToListAsync();
    }

    public async Task<Urun?> GetByIdAsync(int id) =>
        await context.Urunler.AsNoTracking().FirstOrDefaultAsync(u => u.UrunId == id);

    public async Task<Urun> CreateAsync(Urun urun)
    {
        context.Urunler.Add(urun);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Urun eklendi: {urun.UrunId}");
        return urun;
    }

    public async Task UpdateAsync(Urun urun)
    {
        context.Urunler.Update(urun);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Urun guncellendi: {urun.UrunId}");
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await context.Urunler.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        context.Urunler.Remove(entity);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Urun silindi: {id}");
    }
}
