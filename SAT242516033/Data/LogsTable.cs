using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrunSiparisTakip.Models
{
    [Table("LogsTable")]
    public class LogsTable
    {
        [Key]
        public int LogId { get; set; }

        [Required, MaxLength(50)]
        public string LogLevel { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        public string? Exception { get; set; }

        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    }
}
