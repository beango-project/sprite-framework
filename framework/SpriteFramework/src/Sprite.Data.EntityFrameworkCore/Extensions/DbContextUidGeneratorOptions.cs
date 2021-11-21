using System;
using Sprite.UidGenerator;

namespace Sprite.Data.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// DbContext 使用的 Uid 生成器选项配置
    /// </summary>
    public class DbContextUidGeneratorOptions
    {
        public bool IsDistributed { get; private set; }

        protected internal Type UidGeneratorType { get; private set; }

        /// <summary>
        /// 使用Uid生成器
        /// </summary>
        /// <typeparam name="TUidGenerator"></typeparam>
        public void UseUidGenerator<TUidGenerator>() where TUidGenerator : IUniqueIdGenerator
        {
            UidGeneratorType = typeof(TUidGenerator);
            IsDistributed = false;
        }

        /// <summary>
        /// 使用分布式Uid生成器
        /// </summary>
        /// <typeparam name="TUidGenerator"></typeparam>
        public void UseDistributedUidGenerator<TUidGenerator>() where TUidGenerator : IDistributedUniqueIdGenerator
        {
            UidGeneratorType = typeof(TUidGenerator);
            IsDistributed = true;
        }
    }
}