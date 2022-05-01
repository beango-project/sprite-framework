using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sprite.Data.EntityFrameworkCore.Extensions;

namespace Microsoft.EntityFrameworkCore
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