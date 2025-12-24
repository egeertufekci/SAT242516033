using System.Linq;
using Providers;
using SAT242516033.Data;

namespace SAT242516033.Services;

public class KullaniciAuthService
{
    private readonly IMyDbModel_Provider _myDbModelProvider;

    public KullaniciAuthService(IMyDbModel_Provider myDbModelProvider)
    {
        _myDbModelProvider = myDbModelProvider;
    }

    public async Task<Kullanici?> TryLoginAsync(string kullaniciAdi, string sifre)
    {
        if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
        {
            return null;
        }

        var results = await _myDbModelProvider
            .GetItems<Kullanici>("sp_Kullanici_Login",
                ("kullaniciAdi", kullaniciAdi),
                ("sifre", sifre));

        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<Kullanici>> GetKullanicilarAsync()
    {
        return await _myDbModelProvider.GetItems<Kullanici>("sp_Kullanici_List");
    }
}
