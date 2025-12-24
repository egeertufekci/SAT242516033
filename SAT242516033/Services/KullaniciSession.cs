using SAT242516033.Data;

namespace SAT242516033.Services;

public class KullaniciSession
{
    public bool IsAuthenticated => CurrentUser is not null;

    public Kullanici? CurrentUser { get; private set; }

    public void Login(Kullanici kullanici)
    {
        CurrentUser = kullanici;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}
