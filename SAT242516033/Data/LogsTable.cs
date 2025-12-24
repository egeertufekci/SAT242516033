using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("Logs_Table")]
    public class LogsTable
    {
        [Key]
        public int LogId { get; set; }

        [Required, MaxLength(50)]
        public string LogLevel { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Message { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
