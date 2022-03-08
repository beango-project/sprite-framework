namespace Sprite.Data.Persistence
{
    public interface IPersistenceVendorProvider
    {
        IVendor GetPersistenceVendor();
    }
}