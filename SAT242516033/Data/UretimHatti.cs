using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAT242516033.Data;

[Table("UretimHatti")]
public class UretimHatti
{
    [Key]
    public int UretimHattiId { get; set; }

    [Required]
    [MaxLength(50)]
    public string HatKodu { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string HatAdi { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Lokasyon { get; set; }

    public bool AktifMi { get; set; } = true;
}
