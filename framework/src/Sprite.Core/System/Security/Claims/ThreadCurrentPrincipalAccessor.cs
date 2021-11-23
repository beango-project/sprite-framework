using System.Security.Claims;
using System.Threading;
using JetBrains.Annotations;
using Sprite.DependencyInjection;

namespace Sprite.Security.Claims
{
    public class ThreadCurrentPrincipalAccessor : CurrentPrincipalAccessorBase,ISingletonInjection
    {
        [CanBeNull]
        protected override ClaimsPrincipal GetCurrentPrincipal()
        {
            return (ClaimsPrincipal) Thread.CurrentPrincipal?.Identity;
        }
    }
}