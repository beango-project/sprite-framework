using System;
using System.Collections.Generic;
using System.Reflection;
using FastExpressionCompiler.LightExpression;
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
        public static bool HasDefaultId<Tkey>(IEntity<Tkey> entity) where Tkey : IEquatable<Tkey>
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

        public static System.Linq.Expressions.Expression<Func<TEntity, bool>> BuildEntityEqualityExpressionFor<TEntity, TKey>(TKey id)
            where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));
            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.ConstantOf(id)
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam).ToLambdaExpression();

        }

        public static Func<TEntity, bool> EntityEquality<TEntity, TKey>(TKey id)
            where TEntity : IEntity<TKey> where TKey : IEquatable<TKey>
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));
            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TKey))
            );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam).CompileFast();
        }
        
    }
}