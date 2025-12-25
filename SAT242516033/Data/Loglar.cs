using System;
using System.ComponentModel.DataAnnotations; // <-- BU ŞART!

namespace SAT242516033.Data;

public class Loglar
{
    [Key] // Primary Key burada belirtildi
    public int LogID { get; set; }
    public string Seviye { get; set; }
    public string Mesaj { get; set; }
    public DateTime Tarih { get; set; }
    public int? KullaniciID { get; set; }
}