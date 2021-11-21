using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Sprite.Data.Entities;

namespace Sprite.Security.Permissions
{
    public class PermissionGrantDetail : Entity<long>
    {
        [NotNull]
        [Required]
        public virtual string Name { get; protected set; }

        [NotNull]
        [Required]
        public virtual string ProviderKey { get; protected set; }

        [CanBeNull]
        public virtual string ClaimValue { get; protected internal set; }
        
        public PermissionGrantDetail([NotNull] string name, [NotNull] string providerKey, [CanBeNull] string claimValue)
        {
            Name = name;
            ProviderKey = providerKey;
            ClaimValue = claimValue;
        }

    }
}