using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("SiparisDetaylari")]
    public class SiparisDetayi
    {
        [Key]
        public int SiparisDetayId { get; set; }

        [ForeignKey(nameof(Siparis))]
        public int SiparisId { get; set; }

        [ForeignKey(nameof(Urun))]
        public int UrunId { get; set; }

        public int Miktar { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal BirimFiyat { get; set; }

        // Navigations
        public Siparis Siparis { get; set; } = null!;
        public Urun Urun { get; set; } = null!;
    }
}
