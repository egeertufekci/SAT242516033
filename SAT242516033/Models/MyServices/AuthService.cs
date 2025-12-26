using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SAT242516033.Data;
using System.Data;
using System.Threading.Tasks;

namespace SAT242516033.Models.MyServices
{
    public class AuthService
    {
        private readonly string _connectionString;

        public AuthService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Kullanici> LoginAsync(string kadi, string sifre)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                // NOT: Gerçek projede şifreler Hash'li olur. 
                // Şimdilik veritabanında düz tuttuğunu varsayarak basit kontrol yapıyoruz.
                // Eğer Hash kullanıyorsan burayı güncellememiz gerekir.

                string sql = @"
                    SELECT k.KullaniciId, k.KullaniciAdi, k.AdSoyad, k.RolId, r.RolAdi 
                    FROM Kullanici k
                    LEFT JOIN Rol r ON k.RolId = r.RolId
                    WHERE k.KullaniciAdi = @kadi AND k.SifreHash = @sifre";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kadi", kadi);
                cmd.Parameters.AddWithValue("@sifre", sifre);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var rolIdIndex = reader.GetOrdinal("RolId");
                        var rolAdiIndex = reader.GetOrdinal("RolAdi");

                        return new Kullanici
                        {
                            KullaniciId = reader.GetInt32(reader.GetOrdinal("KullaniciId")),
                            KullaniciAdi = reader["KullaniciAdi"].ToString(),
                            AdSoyad = reader["AdSoyad"] == DBNull.Value ? null : reader["AdSoyad"].ToString(),

                            // Rol nesnesi açmıyoruz, direkt değerleri basıyoruz
                            RolId = reader.IsDBNull(rolIdIndex) ? null : reader.GetInt32(rolIdIndex),
                           
                        };
                    }
                }
            }
            return null; // Kullanıcı yoksa null döner
        }
    }
}
