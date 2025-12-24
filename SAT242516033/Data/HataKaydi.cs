using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAT242516033.Data;

[Table("HataKaydi")]
public class HataKaydi
{
    [Key]
    public int HataKaydiId { get; set; }

    public int UretimHattiId { get; set; }

    public int HataTipiId { get; set; }

    public int? KullaniciId { get; set; }

    [MaxLength(30)]
    public string Durum { get; set; } = "Açık";

    public DateTime BildirimTarihi { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Aciklama { get; set; }
}
