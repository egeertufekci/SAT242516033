using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SAT242516033.Data;
using System.Data;
using System.Data.SqlClient;
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
                    SELECT k.KullaniciID, k.KullaniciAdi, k.RolID, r.RolAdi 
                    FROM Kullanici k
                    JOIN Rol r ON k.RolID = r.RolID
                    WHERE k.KullaniciAdi = @kadi AND k.SifreHash = @sifre";

                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kadi", kadi);
                cmd.Parameters.AddWithValue("@sifre", sifre);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Kullanici
                        {
                            KullaniciID = (int)reader["KullaniciID"],
                            KullaniciAdi = reader["KullaniciAdi"].ToString(),

                            // Rol nesnesi açmıyoruz, direkt değerleri basıyoruz
                            RolID = (int)reader["RolID"],
                            RolAdi = reader["RolAdi"].ToString()
                        };
                    }
                }
            }
            return null; // Kullanıcı yoksa null döner
        }
    }
}