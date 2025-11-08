using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("Musteriler")]
    public class Musteri
    {
        [Key]
        public int MusteriId { get; set; }

        [Required, MaxLength(100)]
        public string Ad { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Soyad { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Email { get; set; } = null!;

        // Navigations
        public ICollection<Siparis> Siparisler { get; set; } = new List<Siparis>();
    }
}
