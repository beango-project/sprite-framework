using System.Security.Claims;

namespace System.Security.Claims
{
    public abstract class CurrentPrincipalAccessorBase: ICurrentPrincipalAccessor
    {
        public ClaimsPrincipal CurrentPrincipal => GetCurrentPrincipal();
        protected abstract ClaimsPrincipal GetCurrentPrincipal();
    }
}