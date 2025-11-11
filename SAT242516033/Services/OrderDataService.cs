using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Providers;
using SAT242516033.Components.Pages.Models;
using SAT242516033.Data;
using UrunSiparisTakip.Models;

namespace SAT242516033.Services;

public interface IOrderDataService
{
    Task<DashboardMetrics> GetDashboardMetricsAsync(CancellationToken token = default);
    Task<PaginatedResult<OrderSummaryRow>> GetOrdersAsync(OrderFilterModel filter, CancellationToken token = default);
    Task<IReadOnlyList<OrderDetailRow>> GetOrderDetailsAsync(int siparisId, CancellationToken token = default);
    Task<OrderFormModel?> GetOrderFormAsync(int siparisId, CancellationToken token = default);
    Task<bool> SaveOrderAsync(OrderFormModel form, CancellationToken token = default);
    Task<bool> DeleteOrderAsync(int siparisId, CancellationToken token = default);
    Task<IReadOnlyList<Musteri>> GetCustomersAsync(CancellationToken token = default);
    Task<IReadOnlyList<Urun>> GetProductsAsync(CancellationToken token = default);
    Task<IReadOnlyList<string>> GetDurumOptionsAsync(CancellationToken token = default);
}

public sealed class OrderDataService : IOrderDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.General)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    private readonly IMyDbModel_Provider _provider;
    private readonly ApplicationDbContext _db;

    public OrderDataService(IMyDbModel_Provider provider, ApplicationDbContext db)
    {
        _provider = provider;
        _db = db;
    }

    public async Task<DashboardMetrics> GetDashboardMetricsAsync(CancellationToken token = default)
    {
        var totalOrdersTask = _db.Siparisler.CountAsync(token);
        var pendingOrdersTask = _db.Siparisler.CountAsync(s => s.Durum == "Beklemede", token);
        var totalRevenueTask = _db.SiparisDetaylari
            .Select(d => (decimal?)d.Miktar * d.BirimFiyat)
            .SumAsync(token);
        var activeProductsTask = _db.Urunler.CountAsync(u => u.AktifMi, token);
        var totalCustomersTask = _db.Musteriler.CountAsync(token);

        await Task.WhenAll(totalOrdersTask, pendingOrdersTask, totalRevenueTask, activeProductsTask, totalCustomersTask);

        return new DashboardMetrics
        {
            TotalOrders = totalOrdersTask.Result,
            PendingOrders = pendingOrdersTask.Result,
            TotalRevenue = totalRevenueTask.Result ?? 0m,
            ActiveProducts = activeProductsTask.Result,
            TotalCustomers = totalCustomersTask.Result
        };
    }

    public async Task<PaginatedResult<OrderSummaryRow>> GetOrdersAsync(OrderFilterModel filter, CancellationToken token = default)
    {
        var model = new MyDbModel<OrderSummaryRow>(filter.Page, filter.PageSize, "SiparisId DESC");

        if (filter.MusteriId.HasValue)
        {
            model.Parameters.Where["MusteriId"] = filter.MusteriId.Value.ToString();
        }

        if (!string.IsNullOrWhiteSpace(filter.Durum))
        {
            model.Parameters.Where["Durum"] = filter.Durum!;
        }

        if (filter.StartDate.HasValue)
        {
            model.Parameters.Where["StartDate"] = filter.StartDate.Value.ToString("s");
        }

        if (filter.EndDate.HasValue)
        {
            model.Parameters.Where["EndDate"] = filter.EndDate.Value.AddDays(1).ToString("s");
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            model.Parameters.Where["Search"] = filter.Search!.Trim();
        }

        var result = await _provider.Execute(model, "dbo.sp_Siparisler");
        var items = result.Items.ToList();

        var totalRecords = result.Parameters.TotalRecordCount;
        if (totalRecords == 0 && items.Count > 0)
        {
            totalRecords = items[0].TotalRecordCount;
        }

        return new PaginatedResult<OrderSummaryRow>
        {
            Items = items,
            Page = result.Parameters.PageNumber,
            PageSize = result.Parameters.PageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task<IReadOnlyList<OrderDetailRow>> GetOrderDetailsAsync(int siparisId, CancellationToken token = default)
    {
        var details = await _db.SiparisDetaylari
            .AsNoTracking()
            .Where(d => d.SiparisId == siparisId)
            .OrderBy(d => d.SiparisDetayId)
            .Select(d => new OrderDetailRow
            {
                SiparisDetayId = d.SiparisDetayId,
                SiparisId = d.SiparisId,
                UrunId = d.UrunId,
                UrunAdi = d.Urun.UrunAdi,
                Miktar = d.Miktar,
                BirimFiyat = d.BirimFiyat,
                SatirTutar = d.Miktar * d.BirimFiyat
            })
            .ToListAsync(token);

        return details;
    }

    public async Task<OrderFormModel?> GetOrderFormAsync(int siparisId, CancellationToken token = default)
    {
        var order = await _db.Siparisler
            .AsNoTracking()
            .Include(s => s.Detaylar)
                .ThenInclude(d => d.Urun)
            .FirstOrDefaultAsync(s => s.SiparisId == siparisId, token);

        if (order is null)
        {
            return null;
        }

        return new OrderFormModel
        {
            SiparisId = order.SiparisId,
            MusteriId = order.MusteriId,
            Durum = order.Durum,
            Detaylar = order.Detaylar
                .OrderBy(d => d.SiparisDetayId)
                .Select(d => new OrderFormDetailModel
                {
                    UrunId = d.UrunId,
                    Miktar = d.Miktar,
                    BirimFiyat = d.BirimFiyat
                })
                .ToList()
        };
    }

    public async Task<bool> SaveOrderAsync(OrderFormModel form, CancellationToken token = default)
    {
        if (form.MusteriId is null)
        {
            return false;
        }

        var sanitizedDetails = form.Detaylar
            .Where(d => d.UrunId.HasValue && d.Miktar > 0)
            .Select(d => new
            {
                UrunId = d.UrunId!.Value,
                d.Miktar,
                d.BirimFiyat
            })
            .ToList();

        if (!form.SiparisId.HasValue && sanitizedDetails.Count == 0)
        {
            return false;
        }

        var payload = new
        {
            form.SiparisId,
            MusteriId = form.MusteriId,
            Durum = string.IsNullOrWhiteSpace(form.Durum) ? "Beklemede" : form.Durum,
            Detaylar = sanitizedDetails.Count > 0 ? sanitizedDetails : null
        };

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var operation = form.SiparisId.HasValue ? "update" : "add";

        var response = await _provider.SetItems<OperationAcknowledge>(
            "dbo.sp_Siparis_Add_Update_Remove",
            ("operation", operation),
            ("jsonvalues", json));

        return response.Any(r => r.Value == 1);
    }

    public async Task<bool> DeleteOrderAsync(int siparisId, CancellationToken token = default)
    {
        var payload = JsonSerializer.Serialize(new { SiparisId = siparisId }, JsonOptions);
        var response = await _provider.SetItems<OperationAcknowledge>(
            "dbo.sp_Siparis_Add_Update_Remove",
            ("operation", "remove"),
            ("jsonvalues", payload));

        return response.Any(r => r.Value == 1);
    }

    public async Task<IReadOnlyList<Musteri>> GetCustomersAsync(CancellationToken token = default)
    {
        return await _db.Musteriler
            .AsNoTracking()
            .OrderBy(m => m.Ad)
            .ThenBy(m => m.Soyad)
            .ToListAsync(token);
    }

    public async Task<IReadOnlyList<Urun>> GetProductsAsync(CancellationToken token = default)
    {
        return await _db.Urunler
            .AsNoTracking()
            .OrderBy(u => u.UrunAdi)
            .ToListAsync(token);
    }

    public async Task<IReadOnlyList<string>> GetDurumOptionsAsync(CancellationToken token = default)
    {
        var durumlar = await _db.Siparisler
            .AsNoTracking()
            .Select(s => s.Durum)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(token);

        if (!durumlar.Contains("Beklemede", StringComparer.OrdinalIgnoreCase))
        {
            durumlar.Insert(0, "Beklemede");
        }

        return durumlar;
    }
}
