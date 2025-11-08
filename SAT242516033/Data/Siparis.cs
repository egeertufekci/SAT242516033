using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("Siparisler")]
    public class Siparis
    {
        [Key]
        public int SiparisId { get; set; }

        [ForeignKey(nameof(Musteri))]
        public int MusteriId { get; set; }

        public DateTime SiparisTarihi { get; set; }

        // dbo.DurumType kullanıcı tanımlı tür — burada sade string olarak tutuluyor
        [Required, MaxLength(50)]
        public string Durum { get; set; } = "Beklemede";

        // Navigations
        public Musteri Musteri { get; set; } = null!;
        public ICollection<SiparisDetayi> Detaylar { get; set; } = new List<SiparisDetayi>();
    }
}
