using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Sprite.AspNetCore.Mvc.Conventions
{
    public interface IConventionalRouterBuilder
    {
        string RootPath { get; }

        string ControllerName { get; }

        public ActionModel Action { get; }
        string HttpMethod { get; }

        string Build();
    }
}