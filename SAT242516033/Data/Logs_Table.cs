using System.ComponentModel.DataAnnotations;
using SAT242516033.Models.Attributes;

namespace SAT242516033.Data
{
    public class Logs_Table
    {
        [Key]
        [Sortable(true)]
        [Viewable(true)]
        public int LogId { get; set; }

        [Sortable(true)]
        [Viewable(true)]
        public string TableName { get; set; }

        [Sortable(true)]
        [Viewable(true)]
        public string Operation { get; set; } // INSERT, UPDATE, DELETE

        [Viewable(true)]
        public string? KeyInfo { get; set; }

        [Viewable(false)]
        public string? JsonData { get; set; }

        [Sortable(true)]
        [Viewable(true)]
        public string? UserName { get; set; }

        [Sortable(true)]
        [Viewable(true)]
        public DateTime CreatedAt { get; set; }
    }
}