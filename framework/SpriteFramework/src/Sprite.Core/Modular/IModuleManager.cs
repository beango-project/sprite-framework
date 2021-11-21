namespace Sprite.Modular
{
    public interface IModuleManager
    {
        void InitializeModules(OnApplicationContext context);

        void ShutdownModules(OnApplicationContext context);
    }
}