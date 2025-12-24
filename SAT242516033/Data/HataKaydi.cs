using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("HataKayitlari")]
    public class HataKaydi
    {
        [Key]
        public int HataKaydiId { get; set; }

        public int UretimHattiId { get; set; }
        public int HataTipiId { get; set; }
        public int? KullaniciId { get; set; }

        [MaxLength(1000)]
        public string? Aciklama { get; set; }

        public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? Durum { get; set; }

        public UretimHatti? UretimHatti { get; set; }
        public HataTipi? HataTipi { get; set; }
        public Kullanici? Kullanici { get; set; }
    }
}
