using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("Urunler")]
    public class Urun
    {
        [Key]
        public int UrunId { get; set; }

        [Required, MaxLength(200)]
        public string UrunAdi { get; set; } = null!;

        [Required, MaxLength(64)]
        public string SKU { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal BirimFiyat { get; set; }

        public int StokAdet { get; set; }

        public bool AktifMi { get; set; }

        // Navigations
        public ICollection<SiparisDetayi> SiparisDetaylari { get; set; } = new List<SiparisDetayi>();
        public ICollection<UrunKategori> UrunKategorileri { get; set; } = new List<UrunKategori>();
    }
}
