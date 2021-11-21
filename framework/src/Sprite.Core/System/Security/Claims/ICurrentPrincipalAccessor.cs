using System.Security.Claims;

namespace System.Security.Claims
{
    public interface ICurrentPrincipalAccessor
    {
        ClaimsPrincipal CurrentPrincipal { get; }
    }
}