using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Sprite.Data.EntityFrameworkCore.Extensions;
using Sprite.UidGenerator;

namespace Microsoft.EntityFrameworkCor
{
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// 启用Id自动生成
        /// </summary>
        /// <param name="options"></param>
        public static DbContextOptionsBuilder EnableIdAutoGenerate(this DbContextOptionsBuilder builder, Action<DbContextUidGeneratorOptions> options)
        {
            var dbContextUidGeneratorOptions = new DbContextUidGeneratorOptions();
            options?.Invoke(dbContextUidGeneratorOptions);
            var dbContextOptionsUidGeneratorExtension = new DbContextOptionsUidGeneratorExtension(dbContextUidGeneratorOptions);
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(dbContextOptionsUidGeneratorExtension);
            // builder.LogTo(Console.WriteLine).EnableSensitiveDataLogging();
            // dbContextUidGeneratorOptions.UseDistributedUidGenerator<DistributedUniqueIdGenerator>();
            return builder;
        }

        public static void EnableDataAudit(this DbContextOptionsBuilder builder)
        {
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(new DbContextDataAuditExtension());
        }
    }
}