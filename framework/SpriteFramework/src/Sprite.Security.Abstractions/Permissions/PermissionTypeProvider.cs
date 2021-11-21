using System.Threading.Tasks;

namespace Sprite.Security.Permissions
{
    public abstract class PermissionTypeProvider : IPermissionTypeProvider
    {
        protected PermissionTypeProvider(IPermissionStore store)
        {
            Store = store;
        }

        public abstract IPermissionType Type { get; protected set; }
        public IPermissionStore Store { get; }

        public abstract Task<PermissionGrantedResult> CheckAsync(PermissionCheckContext context);
    }
}