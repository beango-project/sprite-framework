using OpenIddict.Abstractions;

namespace Sprite.Security.OpenIdConnect.Stores
{
    public interface IOpenIdApplicationStore<TApplication> : IOpenIddictApplicationStore<TApplication> where TApplication : class
    {
    }
}