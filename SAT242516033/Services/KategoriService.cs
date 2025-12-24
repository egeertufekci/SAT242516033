using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAT242516033.Data;
using UrunSiparisTakip.Models;

namespace SAT242516033.Services;

public class KategoriService(ApplicationDbContext context, ILogService logService)
{
    public async Task<List<Kategori>> GetAllAsync(bool includeProducts = false)
    {
        var query = context.Kategoriler.AsQueryable();
        if (includeProducts)
        {
            query = query.Include(k => k.UrunKategorileri)
                         .ThenInclude(uk => uk.Urun);
        }

        return await query.AsNoTracking().OrderBy(k => k.KategoriId).ToListAsync();
    }

    public async Task<Kategori?> GetByIdAsync(int id) =>
        await context.Kategoriler.AsNoTracking().FirstOrDefaultAsync(k => k.KategoriId == id);

    public async Task<Kategori> CreateAsync(Kategori kategori)
    {
        context.Kategoriler.Add(kategori);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Kategori eklendi: {kategori.KategoriId}");
        return kategori;
    }

    public async Task UpdateAsync(Kategori kategori)
    {
        context.Kategoriler.Update(kategori);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Kategori guncellendi: {kategori.KategoriId}");
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await context.Kategoriler.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        context.Kategoriler.Remove(entity);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Kategori silindi: {id}");
    }

    public async Task UpdateUrunRelationsAsync(int kategoriId, IEnumerable<int> urunIds)
    {
        var existing = await context.UrunKategorileri
            .Where(uk => uk.KategoriId == kategoriId)
            .ToListAsync();

        var target = urunIds.ToHashSet();

        var toRemove = existing.Where(uk => !target.Contains(uk.UrunId)).ToList();
        if (toRemove.Count > 0)
        {
            context.UrunKategorileri.RemoveRange(toRemove);
        }

        var existingIds = existing.Select(uk => uk.UrunId).ToHashSet();
        var toAdd = target.Where(id => !existingIds.Contains(id)).ToList();
        if (toAdd.Count > 0)
        {
            foreach (var urunId in toAdd)
            {
                context.UrunKategorileri.Add(new UrunKategori
                {
                    KategoriId = kategoriId,
                    UrunId = urunId
                });
            }
        }

        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Kategori urun baglari guncellendi: {kategoriId}");
    }
}
