using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("UrunKategorileri")]
    public class UrunKategori
    {
        public int UrunId { get; set; }
        public int KategoriId { get; set; }

        // Navigations
        public Urun Urun { get; set; } = null!;
        public Kategori Kategori { get; set; } = null!;
    }
}
