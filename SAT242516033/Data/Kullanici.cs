using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAT242516033.Data;

[Table("Kullanici")]
public class Kullanici
{
    [Key]
    public int KullaniciId { get; set; }

    [Required]
    [MaxLength(100)]
    public string KullaniciAdi { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? AdSoyad { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? SifreHash { get; set; }

    public bool AktifMi { get; set; } = true;

    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
}
