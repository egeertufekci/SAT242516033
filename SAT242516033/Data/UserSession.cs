using System.Security.Claims;

namespace SAT242516033.Data
{
    public class UserSession
    {
        public ClaimsPrincipal Principal { get; private set; } = new(new ClaimsIdentity());

        public void SetUser(ClaimsPrincipal principal)
        {
            Principal = principal;
        }
    }
}
