using System;
using JetBrains.Annotations;
using Sprite.Data;

namespace Sprite.MultiTenancy
{
    public class TenantConfiguration<TKey> : ITenantConfiguration<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
        public ConnectionStrings ConnectionStrings { get; set; }

        public TenantConfiguration(TKey id, [NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            Id = id;
            Name = name;

            ConnectionStrings = new ConnectionStrings();
        }
    }
}