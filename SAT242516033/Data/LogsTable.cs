using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAT242516033.Data;

[Table("LogsTable")]
public class LogsTable
{
    [Key]
    public int LogId { get; set; }

    [MaxLength(20)]
    public string LogLevel { get; set; } = "Info";

    public string LogMessage { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Kaynak { get; set; }

    public DateTime LogTarihi { get; set; } = DateTime.UtcNow;

    public int? KullaniciId { get; set; }

    public int? HataKaydiId { get; set; }
}
