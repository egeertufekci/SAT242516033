using System.ComponentModel.DataAnnotations;
using SAT242516033.Models.Attributes;

namespace SAT242516033.Data
{
    public class Siparis
    {
        [Key]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Sipariş No", typeof(SAT242516033.Loc))]
        public int SiparisId { get; set; }

        [Required]
        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Müşteri No", typeof(SAT242516033.Loc))]
        public int MusteriId { get; set; }

        // Gridde Müşteri Adı göstermek istersen Join ile doldurabilirsin, şimdilik ID yeterli

        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Tarih", typeof(SAT242516033.Loc))]
        public DateTime SiparisTarihi { get; set; } = DateTime.Now;

        [Sortable(true)]
        [Viewable(true)]
        [LocalizedDescription("Durum", typeof(SAT242516033.Loc))]
        public string Durum { get; set; } = "Beklemede";

        [Viewable(false)]
        public string? UserId { get; set; }
    }
}