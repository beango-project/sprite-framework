namespace Sprite.AspNetCore.Mvc.Conventions
{
    /// <summary>
    /// 控制器路由
    /// </summary>
    public class ControllerRouterGuide
    {
        public ControllerRouterGuide(string path, string controllerName)
        {
            Path = path;
            ControllerName = controllerName;
        }

        public string Path { get; }

        public string ControllerName { get; }
    }
}