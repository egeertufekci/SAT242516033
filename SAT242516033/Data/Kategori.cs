using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("Kategoriler")]
    public class Kategori
    {
        [Key]
        public int KategoriId { get; set; }

        [Required, MaxLength(150)]
        public string KategoriAdi { get; set; } = null!;

        // Navigations
        public ICollection<UrunKategori> UrunKategorileri { get; set; } = new List<UrunKategori>();
    }
}
