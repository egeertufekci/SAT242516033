using System.ComponentModel.DataAnnotations;
namespace SAT242516033.Data
{
    public class Kullanici
    {
        [System.ComponentModel.DataAnnotations.Key]
        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(false)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("ID", typeof(Loc))]
        public int KullaniciId { get; set; }

        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Ad Soyad", typeof(Loc))]
        public string? AdSoyad { get; set; }

        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Kullanıcı Adı", typeof(Loc))]
        public string? KullaniciAdi { get; set; }

        // Şifreyi Grid'de göstermiyoruz (Viewable=false)
        [SAT242516033.Models.Attributes.Sortable(false)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(false)]
        public string? SifreHash { get; set; }

        [SAT242516033.Models.Attributes.Sortable(false)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(false)]
        public int? RolId { get; set; }

        // Listede Rol Adını göstermek için
        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(false)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Rol", typeof(Loc))]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string? RolAdi { get; set; }

        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(false)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Kayıt Tarihi", typeof(Loc))]
        public DateTime? KayitTarihi { get; set; }
    }
}
