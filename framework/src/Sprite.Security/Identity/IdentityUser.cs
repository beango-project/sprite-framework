using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Sprite.Data.Entities;
using Sprite.Data.Entities.Auditing;

namespace Sprite.Security.Identity
{
    public abstract class IdentityUser<TKey> : Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        // [DisableAuditing]
        public virtual string NormalizedUserName { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Name for the user.
        /// </summary>
        [CanBeNull]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the Surname for the user.
        /// </summary>
        [CanBeNull]
        public virtual string Surname { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the normalized email address for this user.
        /// </summary>
        // [DisableAuditing]
        public virtual string NormalizedEmail { get; protected internal set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public virtual bool EmailConfirmed { get; protected internal set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        // [DisableAuditing]
        public virtual string PasswordHash { get; protected internal set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        // [DisableAuditing]
        public virtual string SecurityStamp { get; protected internal set; }

        public virtual bool IsExternal { get; set; }

        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        [CanBeNull]
        public virtual string PhoneNumber { get; protected internal set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        public virtual bool PhoneNumberConfirmed { get; protected internal set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        public virtual bool TwoFactorEnabled { get; protected internal set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        public virtual DateTimeOffset? LockoutEnd { get; protected internal set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        public virtual bool LockoutEnabled { get; protected internal set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public virtual int AccessFailedCount { get; protected internal set; }

        protected IdentityUser()
        {
        }

        public IdentityUser(string userName, string email)
        {
            UserName = userName;
            NormalizedUserName = userName.ToUpperInvariant();
            Email = email;
            NormalizedEmail = email.ToUpperInvariant();
            SecurityStamp = Guid.NewGuid().ToString();
        }
    }
}