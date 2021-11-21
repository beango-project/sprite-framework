using OpenIddict.Abstractions;

namespace Sprite.Security.OpenIdConnect.Stores
{
    public interface IOpenIdScopeStore<TScope> : IOpenIddictScopeStore<TScope> where TScope : class
    {
        
    }
}