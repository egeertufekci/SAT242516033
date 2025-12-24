using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAT242516033.Data;
using UrunSiparisTakip.Models;

namespace SAT242516033.Services;

public class MusteriService(ApplicationDbContext context, ILogService logService)
{
    public async Task<List<Musteri>> GetAllAsync() =>
        await context.Musteriler.AsNoTracking().OrderBy(m => m.MusteriId).ToListAsync();

    public async Task<Musteri?> GetByIdAsync(int id) =>
        await context.Musteriler.AsNoTracking().FirstOrDefaultAsync(m => m.MusteriId == id);

    public async Task<Musteri> CreateAsync(Musteri musteri)
    {
        context.Musteriler.Add(musteri);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Musteri eklendi: {musteri.MusteriId}");
        return musteri;
    }

    public async Task UpdateAsync(Musteri musteri)
    {
        context.Musteriler.Update(musteri);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Musteri guncellendi: {musteri.MusteriId}");
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await context.Musteriler.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        context.Musteriler.Remove(entity);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Musteri silindi: {id}");
    }
}
