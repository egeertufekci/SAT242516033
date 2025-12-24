using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using UrunSiparisTakip.Models;

namespace SAT242516033.Data
{
    public class SimpleAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly UserSession _session;

        public SimpleAuthenticationStateProvider(UserSession session)
        {
            _session = session;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_session.Principal));
        }

        public Task SignInAsync(Kullanici kullanici)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, kullanici.KullaniciId.ToString()),
                new(ClaimTypes.Name, kullanici.KullaniciAdi)
            };

            if (!string.IsNullOrWhiteSpace(kullanici.Rol))
            {
                claims.Add(new Claim(ClaimTypes.Role, kullanici.Rol));
            }

            var identity = new ClaimsIdentity(claims, "Custom");
            _session.SetUser(new ClaimsPrincipal(identity));
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return Task.CompletedTask;
        }

        public Task SignOutAsync()
        {
            _session.SetUser(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return Task.CompletedTask;
        }
    }
}
