using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sprite.Data.EntityFrameworkCore.Conventions;
using Sprite.Data.Transaction;
using Sprite.UidGenerator;

namespace Sprite.Data.EntityFrameworkCore.Extensions
{
    public class DbContextOptionsUidGeneratorExtension : IDbContextOptionsExtension
    {
        public DbContextUidGeneratorOptions Options => _options;
        private readonly DbContextUidGeneratorOptions _options;

        public DbContextOptionsUidGeneratorExtension(DbContextUidGeneratorOptions options)
        {
            if (options.IsDistributed && options.UidGeneratorType.IsAssignableFrom(typeof(IDistributedUniqueIdGenerator))!)
            {
                throw new ArgumentException($"Given {options.UidGeneratorType} must be {nameof(IUniqueIdGenerator)} or {nameof(IDistributedUniqueIdGenerator)} implement");
            }

            _options = options;
        }

        public void ApplyServices(IServiceCollection services)
        {
        }

        public void Validate(IDbContextOptions options)
        {
        }

        public DbContextOptionsExtensionInfo Info => new DbContextOptionsUidGeneratorExtensionInfo(this);
    }

    public class DbContextOptionsUidGeneratorExtensionInfo : DbContextOptionsExtensionInfo
    {
        public DbContextOptionsUidGeneratorExtensionInfo(IDbContextOptionsExtension instance) : base(instance)
        {
        }

        public override bool IsDatabaseProvider => false;

        public override string LogFragment => "";

        public override int GetServiceProviderHashCode()
        {
            return 0;
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return true;
        }

        public override void PopulateDebugInfo([NotNull] IDictionary<string, string> debugInfo)
        {
        }
    }
}