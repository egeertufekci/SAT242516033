using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Ad", typeof(SAT242516033.Loc))]
        public string? Ad { get; set; }

        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Soyad", typeof(SAT242516033.Loc))]
        public string? Soyad { get; set; }

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
        [NotMapped]
        public string AdSoyad => $"{Ad} {Soyad}".Trim();
    }
}
