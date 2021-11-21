using System;
using Microsoft.EntityFrameworkCore;
using Sprite.Data.EntityFrameworkCore;

namespace Sprite.Security.Identity.EntityFrameworkCore
{
    // public abstract class IdentityDbContextBase<TUser, TRole, TUserKey, TRoleKey, TClaimKey> : DbContextBase<IdentityDbContextBase<TUser, TRole, TUserKey, TRoleKey, TClaimKey>>
    //     where TUser : IdentityUser<TUserKey>
    //     where TRole : IdentityRole<TRoleKey>
    //     where TClaimKey : IEquatable<TClaimKey>
    //     where TUserKey : IEquatable<TUserKey>
    //     where TRoleKey : IEquatable<TRoleKey>
    // {
    //     protected IdentityDbContextBase(DbContextOptions<IdentityDbContextBase<TUser, TRole, TUserKey, TRoleKey, TClaimKey>> options) : base(options)
    //     {
    //     }
    //
    //     public DbSet<TUser> Users { get; }
    //
    //     public DbSet<TRole> Roles { get; }
    // }

    public class IdentityDbContext<TUser, TUserKey, TRole, TRoleKey, TUserRole, TUserLogin, TUserLoginKey, TUserToken, TUserTokenKey, TUserClaim,
        TUserClaimKey, TRoleClaim, TRoleClaimKey> : DbContextBase<IdentityDbContext<TUser, TUserKey, TRole, TRoleKey, TUserRole, TUserLogin, TUserLoginKey,
        TUserToken, TUserTokenKey, TUserClaim,
        TUserClaimKey, TRoleClaim, TRoleClaimKey>>
        where TUser : IdentityUser<TUserKey>, new()
        where TUserKey : IEquatable<TUserKey>
        where TRole : IdentityRole<TRoleKey>, new()
        where TRoleKey : IEquatable<TRoleKey>
        where TUserRole : IdentityUserRole<TUserKey, TRoleKey>, new()
        where TUserLogin : IdentityUserLogin<TUserLoginKey, TUserKey>, new()
        where TUserLoginKey : IEquatable<TUserLoginKey>
        where TUserToken : IdentityUserToken<TUserTokenKey, TUserKey>, new()
        where TUserTokenKey : IEquatable<TUserTokenKey>
        where TUserClaim : IdentityUserClaim<TUserClaimKey, TUserKey>, new()
        where TUserClaimKey : IEquatable<TUserClaimKey>
        where TRoleClaim : IdentityRoleClaim<TRoleClaimKey, TRoleKey>, new()
        where TRoleClaimKey : IEquatable<TRoleClaimKey>
    {
        public IdentityDbContext(
            DbContextOptions<IdentityDbContext<TUser, TUserKey, TRole, TRoleKey, TUserRole, TUserLogin, TUserLoginKey, TUserToken, TUserTokenKey, TUserClaim,
                TUserClaimKey, TRoleClaim, TRoleClaimKey>> options) : base(options)
        {
        }

        public DbSet<TUser> Users { get; }

        public DbSet<TRole> Roles { get; }

        public DbSet<TUserRole> UserRoles { get; }

        public DbSet<TUserLogin> UserLogins { get; }

        public DbSet<TUserToken> UserTokens { get; }

        public DbSet<TUserClaim> UserClaims { get; }

        public DbSet<TRoleClaim> RoleClaims { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<TUserRole>(builder =>
            // {
            //     builder.HasIndex(ur => new { ur.RoleId, ur.UserId, });
            //     builder.Navigation(x => x.User);
            //     builder.Navigation(x => x.Role);
            // });

            base.OnModelCreating(modelBuilder);
        }
    }

    // public abstract class IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> : DbContextBase<
    //     IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>
    //     where TUser : IdentityUser<TKey>
    //     where TRole : IdentityRole<TKey>
    //     where TKey : IEquatable<TKey>
    //     where TUserClaim : IdentityUserClaim<TKey, TKey>
    //     where TUserRole : IdentityUserRole<TKey, TKey, TKey>
    //     where TUserLogin : IdentityUserLogin<TKey, TKey>
    //     where TRoleClaim : IdentityRoleClaim<TKey, TKey>
    //     where TUserToken : IdentityUserToken<TKey, TKey>
    // {
    //     protected IdentityDbContext(DbContextOptions<IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>> options) : base(options)
    //     {
    //     }
    // }
}