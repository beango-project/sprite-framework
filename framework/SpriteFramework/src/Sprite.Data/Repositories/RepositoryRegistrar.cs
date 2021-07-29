using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sprite.Data.DependencyInjection;
using Sprite.Data.Entities;

namespace Sprite.Data.Repositories
{
    /// <summary>
    /// 仓储注册器
    /// </summary>
    public abstract class RepositoryRegistrar<TOptions>
        where TOptions : PersistenceVenderRegistrationOptions
    {
        protected RepositoryRegistrar(TOptions options)
        {
            Options = options;
        }

        public TOptions Options { get; }


        /// <summary>
        /// 注册所有仓储
        /// </summary>
        public virtual void RegistrarRepositories()
        {
            if (Options.IsUseDefaultRepositories)
            {
                RegistrarDefaultRepositories();
            }
        }

        /// <summary>
        /// 注册默认仓储
        /// </summary>
        protected virtual void RegistrarDefaultRepositories()
        {
            foreach (var entityType in GetEntityTypes(Options.OriginalPersistenceVenderType))
            {
                RegistrarDefaultRepository(entityType);
            }
        }

        /// <summary>
        /// 注册默认仓储
        /// </summary>
        /// <param name="entityType">实体类型</param>
        protected virtual void RegistrarDefaultRepository(Type entityType)
        {
            Options.Services.AddDefaultRepository(entityType, GetDefaultRepositoryImplementationType(entityType));
        }

        /// <summary>
        /// 获取默认仓储的实现类型
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>仓储实现类型</returns>
        protected virtual Type GetDefaultRepositoryImplementationType(Type entityType)
        {
            var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);

            if (primaryKeyType == null)
            {
                return Options.SpecifiedDefaultRepositoryTypes
                    ? Options.DefaultRepositoryImplementationTypeWithoutKey.MakeGenericType(entityType)
                    : GetRepositoryType(Options.DefaultRepositoryPersistenceVenderType, entityType);
            }

            return Options.SpecifiedDefaultRepositoryTypes
                ? Options.DefaultRepositoryImplementationType.MakeGenericType(entityType, primaryKeyType)
                : GetRepositoryType(Options.DefaultRepositoryPersistenceVenderType, entityType, primaryKeyType);
        }

        /// <summary>
        /// 获取实体的类型
        /// </summary>
        /// <param name="persistenceVenderType"></param>
        /// <returns></returns>
        protected abstract IEnumerable<Type> GetEntityTypes(Type persistenceVenderType);

        protected abstract Type GetRepositoryType(Type persistenceVenderType, Type entityType);

        protected abstract Type GetRepositoryType(Type persistenceVenderType, Type entityType, Type primaryKeyType);
    }
}