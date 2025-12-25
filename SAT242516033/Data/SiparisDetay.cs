using System.ComponentModel.DataAnnotations;
using SAT242516033.Models.Attributes;

namespace SAT242516033.Data
{
    public class SiparisDetay
    {
        [Key]
        [Sortable(true)]
        [Viewable(true)]
        public int SiparisDetayId { get; set; }

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        public int SiparisId { get; set; }

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        public int UrunId { get; set; }

        // Ürün Adını listede göstermek için (View veya Join ile dolar)
        [Viewable(true)]
        public string? UrunAdi { get; set; }

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        public int Miktar { get; set; } = 1;

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        public decimal BirimFiyat { get; set; }

        // Veritabanında yok ama hesaplama için lazım
        public decimal ToplamTutar => Miktar * BirimFiyat;
    }
}