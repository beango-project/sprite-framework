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
        /// 开启Id自动生成器
        /// </summary>
        /// <param name="options"></param>
        public static void EnableIdAutoGenerator(this DbContextOptionsBuilder builder, Action<DbContextUidGeneratorOptions> options)
        {
            var dbContextUidGeneratorOptions = new DbContextUidGeneratorOptions();
            options?.Invoke(dbContextUidGeneratorOptions);
            var dbContextOptionsUidGeneratorExtension = new DbContextOptionsUidGeneratorExtension(dbContextUidGeneratorOptions);
            
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(dbContextOptionsUidGeneratorExtension);
            // builder.LogTo(Console.WriteLine).EnableSensitiveDataLogging();
            dbContextUidGeneratorOptions.UseDistributedUidGenerator<DistributedUniqueIdGenerator>();
        }
    }
}