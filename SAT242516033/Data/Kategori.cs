using System.ComponentModel.DataAnnotations;
using SAT242516033.Models.Attributes;

namespace SAT242516033.Data
{
    public class Kategori
    {
        [Key]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("ID", typeof(SAT242516033.Loc))]
        public int KategoriId { get; set; }

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Kategori Adı", typeof(SAT242516033.Loc))]
        public string KategoriAdi { get; set; }
    }
}