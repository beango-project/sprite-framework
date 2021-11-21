using OpenIddict.Abstractions;

namespace Sprite.Security.OpenIdConnect.Stores
{
    public interface IOpenIdTokenStore<TToken> : IOpenIddictTokenStore<TToken> where TToken : class
    {
    }
}