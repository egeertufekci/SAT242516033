using System.ComponentModel.DataAnnotations;
using SAT242516033.Models.Attributes;

namespace SAT242516033.Data
{
    public class Urun
    {
        [Key]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("ID", typeof(SAT242516033.Loc))]
        public int UrunId { get; set; }

        [Required(ErrorMessage = "Ürün adı giriniz.")]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Ürün Adı", typeof(SAT242516033.Loc))]
        public string UrunAdi { get; set; }

        // Veritabanında ilişki için ID tutuyoruz
        [Sortable(true)]
        [Viewable(false)]
        public int? KategoriId { get; set; }

        // Ekranda ID yerine İsim görmek için (SQL Join ile dolacak)
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Kategori", typeof(SAT242516033.Loc))]
        public string? KategoriAdi { get; set; }

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Fiyat", typeof(SAT242516033.Loc))]
        public decimal BirimFiyat { get; set; }

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Stok", typeof(SAT242516033.Loc))]
        public int StokAdet { get; set; }

        [Viewable(true)]
        [LocalizedDescription("Aktif", typeof(SAT242516033.Loc))]
        public bool AktifMi { get; set; } = true;
    }
}