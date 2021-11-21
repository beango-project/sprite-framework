using System;
using System.Diagnostics;
using Sprite.Data.Entities;

namespace Sprite.Security.OpenIdConnect.Models
{
    [DebuggerDisplay("Id = {Id.ToString(),nq} ; Name = {Name,nq}")]
    public class OpenIdScope<TKey> : Entity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the concurrency token.
        /// </summary>
        public virtual string? ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the public description associated with the current scope.
        /// </summary>
        public virtual string? Description { get; set; }

        /// <summary>
        /// Gets or sets the localized public descriptions associated
        /// with the current scope, serialized as a JSON object.
        /// </summary>
        public virtual string? Descriptions { get; set; }

        /// <summary>
        /// Gets or sets the display name associated with the current scope.
        /// </summary>
        public virtual string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the localized display names
        /// associated with the current application,
        /// serialized as a JSON object.
        /// </summary>
        public virtual string? DisplayNames { get; set; }
        
        /// <summary>
        /// Gets or sets the unique name associated with the current scope.
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        /// Gets or sets the additional properties serialized as a JSON object,
        /// or <c>null</c> if no bag was associated with the current scope.
        /// </summary>
        public virtual string? Properties { get; set; }

        /// <summary>
        /// Gets or sets the resources associated with the
        /// current scope, serialized as a JSON array.
        /// </summary>
        public virtual string? Resources { get; set; }
    }
}