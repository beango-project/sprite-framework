using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sprite.Data.DependencyInjection
{
    public class PersistenceVenderRegistrationOptions
    {
        public PersistenceVenderRegistrationOptions(IServiceCollection services, Type originalPersistenceVenderType)
        {
            Services = services;
            OriginalPersistenceVenderType = originalPersistenceVenderType;
            DefaultRepositoryPersistenceVenderType = originalPersistenceVenderType;
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// 原始持久化提供商类型
        /// </summary>
        public Type OriginalPersistenceVenderType { get; }

        /// <summary>
        /// 默认仓储持久化提供商类型
        /// </summary>
        public Type DefaultRepositoryPersistenceVenderType { get; protected set; }

        /// <summary>
        /// 默认仓储实现类型
        /// </summary>
        public Type DefaultRepositoryImplementationType { get; private set; }

        /// <summary>
        /// 默认仓储
        /// </summary>
        public Type DefaultRepositoryImplementationTypeWithoutKey { get; private set; }

        /// <summary>
        /// 是否使用默认仓储
        /// </summary>
        public bool IsUseDefaultRepositories { get; private set; }


        public bool SpecifiedDefaultRepositoryTypes => DefaultRepositoryImplementationType != null && DefaultRepositoryImplementationTypeWithoutKey != null;

        public void AddDefaultRepositories(bool includeAllEntities = false)
        {
            IsUseDefaultRepositories = true;
        }
    }
}