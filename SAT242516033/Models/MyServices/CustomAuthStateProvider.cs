using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace SAT242516033.Models.MyServices
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        // Constructor'da SessionStorage istiyoruz (Program.cs'e eklemiştik)
        public CustomAuthStateProvider(ProtectedSessionStorage sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Tarayıcı hafızasını oku
                var userSessionResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;

                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                // Hafızada bilgi varsa kimliği oluştur
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role)
                }, "CustomAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                // Giriş yapılıyor -> Hafızaya YAZ
                await _sessionStorage.SetAsync("UserSession", userSession);
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role)
                }, "CustomAuth"));
            }
            else
            {
                // Çıkış yapılıyor -> Hafızadan SİL
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }

    // Bu sınıfı da dosyanın en altına ekle
    public class UserSession
    {
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}