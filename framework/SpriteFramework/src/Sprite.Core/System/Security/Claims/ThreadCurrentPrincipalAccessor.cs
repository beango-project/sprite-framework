using System.Security.Claims;
using System.Threading;
using Sprite.DependencyInjection;

namespace Sprite.Security.Claims
{
    public class ThreadCurrentPrincipalAccessor : CurrentPrincipalAccessorBase,ISingletonInjection
    {
        protected override ClaimsPrincipal GetCurrentPrincipal()
        {
            return (ClaimsPrincipal) Thread.CurrentPrincipal?.Identity;
        }
    }
}