using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("HataTipleri")]
    public class HataTipi
    {
        [Key]
        public int HataTipiId { get; set; }

        [Required, MaxLength(150)]
        public string HataAdi { get; set; } = null!;

        [MaxLength(500)]
        public string? Aciklama { get; set; }

        public bool AktifMi { get; set; } = true;

        public ICollection<HataKaydi> HataKayitlari { get; set; } = new List<HataKaydi>();
    }
}
