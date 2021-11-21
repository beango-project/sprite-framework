using OpenIddict.Abstractions;

namespace Sprite.Security.OpenIdConnect.Stores
{
    public interface IOpenIdAuthorizationStore<TAuthorization> : IOpenIddictAuthorizationStore<TAuthorization> where TAuthorization : class
    {
    }
}