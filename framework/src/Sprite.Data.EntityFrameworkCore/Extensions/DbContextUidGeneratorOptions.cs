using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ImTools;
using Sprite.UidGenerator;

namespace Sprite.Data.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// DbContext 使用的 Uid 生成器选项配置
    /// </summary>
    public class DbContextUidGeneratorOptions
    {
         public bool Enable { get; set; }
         
        protected internal Type UidGeneratorType { get; private set; }

        /// <summary>
        /// 使用类型，生成器类型，是否跳过
        /// </summary>
        private SortedDictionary<Type, List<KeyValuePair<Type, bool>>> TypeAndUiGenerator { get; }

        /// <summary>
        /// 使用Uid生成器
        /// </summary>
        /// <typeparam name="TUidGenerator"></typeparam>
        public void UseUidGenerator<TUidGenerator>() where TUidGenerator : IUniqueIdGenerator
        {
            UidGeneratorType = typeof(TUidGenerator);
        }

        /// <summary>
        /// 使用分布式Uid生成器
        /// </summary>
        /// <typeparam name="TUidGenerator"></typeparam>
        public void UseDistributedUidGenerator<TUidGenerator>(bool ignore = false) where TUidGenerator : IDistributedUniqueIdGenerator
        {
            UidGeneratorType = typeof(TUidGenerator);
        }

        /// <summary>
        /// 使用分布式Uid生成器
        /// </summary>
        /// <typeparam name="TUidGenerator"></typeparam>
        public void UseDistributedUidGenerator<TUidGenerator, T>() where TUidGenerator : IDistributedUniqueIdGenerator<T>
        {
            UidGeneratorType = typeof(TUidGenerator);
        }
    }
}