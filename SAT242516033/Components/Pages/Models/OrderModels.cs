using System.ComponentModel.DataAnnotations;

namespace SAT242516033.Components.Pages.Models;

public class OrderSummaryRow
{
    public int SiparisId { get; set; }
    public DateTime SiparisTarihi { get; set; }
    public string? Musteri { get; set; }
    public string? Durum { get; set; }
    public decimal ToplamTutar { get; set; }
    public int TotalRecordCount { get; set; }
}

public class OrderDetailRow
{
    public int SiparisDetayId { get; set; }
    public int SiparisId { get; set; }
    public int UrunId { get; set; }
    public string? UrunAdi { get; set; }
    public int Miktar { get; set; }
    public decimal BirimFiyat { get; set; }
    public decimal SatirTutar { get; set; }
}

public class OrderFilterModel
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 10;

    public int? MusteriId { get; set; }
    public string? Durum { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Search { get; set; }
}

public class OrderFormDetailModel
{
    [Required(ErrorMessage = "Ürün seçilmelidir.")]
    public int? UrunId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Miktar 1 veya daha fazla olmalıdır.")]
    public int Miktar { get; set; } = 1;

    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Birim fiyat sıfırdan büyük olmalıdır.")]
    public decimal BirimFiyat { get; set; }

    public decimal SatirTutar => Math.Round(Miktar * BirimFiyat, 2);
}

public class OrderFormModel
{
    public int? SiparisId { get; set; }

    [Required(ErrorMessage = "Müşteri seçimi zorunludur.")]
    public int? MusteriId { get; set; }

    [Required, StringLength(50)]
    public string? Durum { get; set; } = "Beklemede";

    public List<OrderFormDetailModel> Detaylar { get; set; } = new();

    public bool HasValidDetail => Detaylar.Any();
}

public class OperationAcknowledge
{
    public string? Key { get; set; }
    public int Value { get; set; }
}

public class PaginatedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalRecords { get; init; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);
    public bool HasResults => Items.Count > 0;
}

public class DashboardMetrics
{
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public decimal TotalRevenue { get; init; }
    public int ActiveProducts { get; init; }
    public int TotalCustomers { get; init; }
}
