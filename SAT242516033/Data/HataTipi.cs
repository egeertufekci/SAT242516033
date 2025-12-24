using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAT242516033.Data;

[Table("HataTipi")]
public class HataTipi
{
    [Key]
    public int HataTipiId { get; set; }

    [Required]
    [MaxLength(50)]
    public string HataKodu { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string HataAdi { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Aciklama { get; set; }

    public int KritikSeviye { get; set; } = 1;
}
