using System.ComponentModel.DataAnnotations;
using SAT242516033.Models.Attributes;

namespace SAT242516033.Data
{
    public class Musteri
    {
        [Key]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("ID", typeof(SAT242516033.Loc))]
        public int MusteriId { get; set; }

        [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Ad", typeof(SAT242516033.Loc))]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Soyad", typeof(SAT242516033.Loc))]
        public string Soyad { get; set; }

        [Viewable(true)]
        [LocalizedDescription("E-Posta", typeof(SAT242516033.Loc))]
        public string? Email { get; set; }

        [Viewable(true)]
        [LocalizedDescription("Telefon", typeof(SAT242516033.Loc))]
        public string? Telefon { get; set; }

        [Viewable(false)] // Listede çok yer kaplamasın diye gizli
        [LocalizedDescription("Adres", typeof(SAT242516033.Loc))]
        public string? Adres { get; set; }

        // Grid veya Dropdownlarda Ad Soyad beraber gözüksün diye
        public string AdSoyad => $"{Ad} {Soyad}";
    }
}