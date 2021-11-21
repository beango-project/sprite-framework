using Sprite.Web.Http.Models;

namespace Sprite.Web.Http.ProxyScripts.Generators
{
    public interface IProxyScriptGenerator
    {
        string CreateScript(ApplicationApiDefinition model);
    }
}