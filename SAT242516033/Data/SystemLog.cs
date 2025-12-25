using System.ComponentModel.DataAnnotations;

namespace SAT242516033.Data
{
    // Veritabanındaki 'Logs' tablosunun karşılığı
    public class SystemLog
    {
        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("ID", typeof(Loc))]
        public int Id { get; set; }

        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Zaman", typeof(Loc))]
        public DateTime Timestamp { get; set; }

        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Seviye", typeof(Loc))]
        public string? Level { get; set; }

        [SAT242516033.Models.Attributes.Sortable(true)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Kategori", typeof(Loc))]
        public string? Category { get; set; }

        [SAT242516033.Models.Attributes.Sortable(false)]
        [SAT242516033.Models.Attributes.Viewable(true)]
        [SAT242516033.Models.Attributes.LocalizedDescription("Mesaj", typeof(Loc))]
        public string? Message { get; set; }

        public string? Exception { get; set; }
    }
}