using System;
using System.Collections.Generic;
using Sprite.Data.Entities;
using Sprite.Data.Entities.Auditing;

namespace Sprite.Security.OpenIdConnect.Models
{
    
    public class OpenIdAuthorization<TKey> : OpenIdAuthorization<TKey, OpenIdApplication<TKey>, OpenIdToken<TKey>>
        where TKey : notnull, IEquatable<TKey>
    {
    }

    
    public class OpenIdAuthorization<TKey, TApplication,  TToken> : Entity<TKey>
        where TKey : notnull, IEquatable<TKey>
        where TApplication : class
        where TToken : class
    {
        /// <summary>
        /// Gets or sets the application associated with the current authorization.
        /// </summary>
        public virtual TApplication? Application { get; set; }

        /// <summary>
        /// Gets or sets the concurrency token.
        /// </summary>
        public virtual string? ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the UTC creation date of the current authorization.
        /// </summary>
        [CreatedDate]
        public virtual DateTime? CreationDate { get; set; }
        
        /// <summary>
        /// Gets or sets the unique identifier associated with the current authorization.
        /// </summary>
        public override TKey Id { get; protected set; }

        /// <summary>
        /// Gets or sets the additional properties serialized as a JSON object,
        /// or <c>null</c> if no bag was associated with the current authorization.
        /// </summary>
        public virtual string? Properties { get; set; }

        /// <summary>
        /// Gets or sets the scopes associated with the current
        /// authorization, serialized as a JSON array.
        /// </summary>
        public virtual string? Scopes { get; set; }

        /// <summary>
        /// Gets or sets the status of the current authorization.
        /// </summary>
        public virtual string? Status { get; set; }

        /// <summary>
        /// Gets or sets the subject associated with the current authorization.
        /// </summary>
        public virtual string? Subject { get; set; }

        /// <summary>
        /// Gets the list of tokens associated with the current authorization.
        /// </summary>
        public virtual ICollection<TToken> Tokens { get; } = new HashSet<TToken>();

        /// <summary>
        /// Gets or sets the type of the current authorization.
        /// </summary>
        public virtual string? Type { get; set; }
    }
}