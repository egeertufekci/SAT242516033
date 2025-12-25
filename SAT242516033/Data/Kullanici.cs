using System.ComponentModel.DataAnnotations;
namespace SAT242516033.Data
{
    public class Kullanici
    {
        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(false)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("ID", typeof(Loc))]
        public int KullaniciID { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Kullanıcı Adı", typeof(Loc))]
        public string KullaniciAdi { get; set; }

        // Şifreyi Grid'de göstermiyoruz (Viewable=false)
        [SAT242516033.Models.Attributes.Sortable(false)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(false)]
        public string? SifreHash { get; set; }

        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        [SAT242516033.Models.Attributes.Sortable(false)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(false)]
        public int RolID { get; set; }

        // Listede Rol Adını göstermek için
        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(false)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Rol", typeof(Loc))]
        public string? RolAdi { get; set; }
    }
}