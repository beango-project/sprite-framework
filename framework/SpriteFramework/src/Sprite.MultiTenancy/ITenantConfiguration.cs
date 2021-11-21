using System;
using Sprite.Data;

namespace Sprite.MultiTenancy
{
    public interface ITenantConfiguration<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public string Name { get; set; }

        public ConnectionStrings ConnectionStrings { get; set; }
    }
}