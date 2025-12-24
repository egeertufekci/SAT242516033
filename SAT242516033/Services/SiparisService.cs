using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SAT242516033.Data;
using UrunSiparisTakip.Models;

namespace SAT242516033.Services;

public class SiparisService(ApplicationDbContext context, ILogService logService)
{
    public async Task<List<Siparis>> GetAllAsync() =>
        await context.Siparisler
            .Include(s => s.Musteri)
            .Include(s => s.Detaylar)
            .ThenInclude(d => d.Urun)
            .AsNoTracking()
            .OrderByDescending(s => s.SiparisTarihi)
            .ToListAsync();

    public async Task<Siparis?> GetByIdAsync(int id) =>
        await context.Siparisler
            .Include(s => s.Musteri)
            .Include(s => s.Detaylar)
            .ThenInclude(d => d.Urun)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SiparisId == id);

    public async Task<int> CreateAsync(int musteriId, string durum, IEnumerable<SiparisDetayi> detaylar)
    {
        var siparis = new Siparis
        {
            MusteriId = musteriId,
            Durum = string.IsNullOrWhiteSpace(durum) ? "Beklemede" : durum,
            SiparisTarihi = DateTime.UtcNow,
            Detaylar = detaylar.ToList()
        };

        context.Siparisler.Add(siparis);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Siparis olusturuldu: {siparis.SiparisId}");
        return siparis.SiparisId;
    }

    public async Task UpdateAsync(Siparis siparis)
    {
        context.Siparisler.Update(siparis);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Siparis guncellendi: {siparis.SiparisId}");
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await context.Siparisler.FindAsync(id);
        if (entity == null)
        {
            return;
        }

        context.Siparisler.Remove(entity);
        await context.SaveChangesAsync();
        await logService.LogAsync("Info", $"Siparis silindi: {id}");
    }
}
