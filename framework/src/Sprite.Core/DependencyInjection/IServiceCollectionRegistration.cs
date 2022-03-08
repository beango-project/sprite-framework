namespace Sprite.DependencyInjection
{
    public interface IServiceCollectionRegistration
    {
        void Add(ServiceRegistration serviceRegistration);

        void Replace(ServiceRegistration serviceRegistration);

        void Get(ServiceRegistration serviceRegistration);
    }
}