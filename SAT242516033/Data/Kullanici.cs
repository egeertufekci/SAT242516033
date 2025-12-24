using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("Kullanicilar")]
    public class Kullanici
    {
        [Key]
        public int KullaniciId { get; set; }

        [Required, MaxLength(100)]
        public string KullaniciAdi { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Sifre { get; set; } = null!;

        [MaxLength(100)]
        public string? Ad { get; set; }

        [MaxLength(100)]
        public string? Soyad { get; set; }

        [MaxLength(50)]
        public string? Rol { get; set; }

        public bool AktifMi { get; set; } = true;

        public ICollection<HataKaydi> HataKayitlari { get; set; } = new List<HataKaydi>();
    }
}
