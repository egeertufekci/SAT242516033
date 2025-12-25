using System.ComponentModel.DataAnnotations;
namespace SAT242516033.Data
{
    public class Rol
    {
        [Key]
        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(false)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("ID", typeof(Loc))]
        public int RolId { get; set; }

        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Editable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Rol Adı", typeof(Loc))]
        public string? RolAdi { get; set; }
    }
}
