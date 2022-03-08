namespace Sprite.Data.Persistence
{
    public interface IPersistenceVendoProvider
    {
        IVendor GetPersistenceVendor();
    }
}