using System.Linq;
using JetBrains.Annotations;
using Sprite;

namespace System.Security.Claims
{
    public static class ClaimsIdentityExtensions
    {
        [CanBeNull]
        #nullable enable
        public static Claim? GetCurrentUser([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));
            if (!IsAuthenticated(principal))
            {
                return null;
            }

            var id = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (id == null || id.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return id;
        }

        // public static T? GetCurrentUserId<T>([NotNull] this ClaimsPrincipal principal)
        // {
        //     Check.NotNull(principal, nameof(principal));
        //     if (IsAuthenticated(principal))
        //     {
        //         return new T?();
        //     }
        //
        //     var id = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        //     if (id == null || id.Value.IsNullOrWhiteSpace())
        //     {
        //         return new T?();
        //     }
        //
        //     var changeValue = (T)Convert.ChangeType(id, typeof(T));
        //     return changeValue;
        // }

        public static bool IsAuthenticated([NotNull] this ClaimsPrincipal principal)
        {
            Check.NotNull(principal, nameof(principal));
            return principal.Identity?.IsAuthenticated ?? false;
        }
    }
}