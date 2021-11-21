namespace Sprite.Modular
{
    public interface IModuleShutdownProcessor : IModuleProcessor
    {
        void Shutdown(OnApplicationContext context);
    }
}