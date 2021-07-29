namespace Sprite.Data.Persistence
{
    public abstract class PersistenceVendorAdapter<TVendor> where TVendor : class
    {
        private TVendor _vendor;

        public PersistenceVendorAdapter(TVendor vendor)
        {
            _vendor = vendor;
        }
    }
}