using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace Sprite.Data.Entities
{
    public static class EntityHelper
    {
        /// <summary>
        /// 是否有Id
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool HasDefaultId<Tkey>(IEntity<Tkey> entity)
        {
            if (EqualityComparer<Tkey>.Default.Equals(entity.Id, default))
            {
                return true;
            }

            if (typeof(Tkey) == typeof(int))
            {
                return Convert.ToInt32(entity.Id) <= 0;
            }

            if (typeof(Tkey) == typeof(long))
            {
                return Convert.ToInt64(entity.Id) <= 0;
            }

            return false;
        }

        [CanBeNull]
        public static Type FindPrimaryKeyType([NotNull] Type entityType)
        {
            if (!typeof(IEntity).IsAssignableFrom(entityType))
            {
                throw new Exception(
                    $"Given {nameof(entityType)} is not an entity. It should implement {typeof(IEntity).AssemblyQualifiedName}!");
            }

            foreach (var interfaceType in entityType.GetTypeInfo().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IEntity<>))
                {
                    return interfaceType.GenericTypeArguments[0];
                }
            }

            return null;
        }

        public static Expression<Func<TEntity, bool>> BuildEntityEqualityExpressionFor<TEntity, TKey>(TKey id)
            where TEntity : IEntity<TKey>
        {
            var lambdaParam = FastExpressionCompiler.LightExpression.Expression.Parameter(typeof(TEntity));
            var lambdaBody = FastExpressionCompiler.LightExpression.Expression.Equal(
                FastExpressionCompiler.LightExpression.Expression.PropertyOrField(lambdaParam, "Id"),
                FastExpressionCompiler.LightExpression.Expression.Constant(id, typeof(TKey))
            );

            return FastExpressionCompiler.LightExpression.Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam).ToExpression() as Expression<Func<TEntity, bool>>;
        }
    }
}