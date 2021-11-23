using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using FastExpressionCompiler.LightExpression;
using JetBrains.Annotations;
using Sprite;

namespace System.Linq
{
    public static class FilterHelper
    {
        private static readonly Dictionary<FilterOperation, Func<Expression, Expression, Expression>> ExpressionDict =
            new Dictionary<FilterOperation, Func<Expression, Expression, Expression>>
            {
                {
                    FilterOperation.Equal, Expression.Equal
                },
                {
                    FilterOperation.NotEqual, Expression.NotEqual
                },
                {
                    FilterOperation.LessThan, Expression.LessThan
                },
                {
                    FilterOperation.GreaterThan, Expression.GreaterThan
                },
                {
                    FilterOperation.LessThanOrEqual, Expression.LessThanOrEqual
                },
                {
                    FilterOperation.GreaterThanOrEqual, Expression.GreaterThanOrEqual
                },
                {
                    FilterOperation.StartsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“StartsWith”比较方式只支持字符串类型的数据");
                        }

                        return Expression.Call(left, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                            right);
                    }
                },
                {
                    FilterOperation.EndsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“EndsWith”比较方式只支持字符串类型的数据");
                        }

                        return Expression.Call(left, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                            right);
                    }
                },
                {
                    FilterOperation.Contains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("“Contains”比较方式只支持字符串类型的数据");
                        }

                        return Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                            right);
                    }
                },
                // {
                //     FilterOperation.NotContains,
                //     (left, right) =>
                //     {
                //         if (left.Type != typeof(string))
                //         {
                //             throw new NotSupportedException("“Contains”比较方式只支持字符串类型的数据");
                //         }
                //
                //         return Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                //             right);
                //     }
                // }
                //{
                //    FilterOperates.StdIn, (left, right) =>
                //    {
                //        if (!right.Type.IsArray)
                //        {
                //            return null;
                //        }
                //        return left.Type != typeof (string) ? null : Expression.Call(typeof (Enumerable), "Contains", new[] {left.Type}, right, left);
                //    }
                //},
                //{
                //    FilterOperates.DataTimeLessThanOrEqual, Expression.LessThanOrEqual
                //}
            };


        /// <summary>
        ///     获取指定查询条件组的查询表达式
        /// </summary>
        /// <typeparam name="T">表达式实体类型</typeparam>
        /// <param name="group">查询条件组，如果为null，则直接返回 true 表达式</param>
        public static Expression<Func<T, bool>> GetExpression<T>(FilterGroup group)
        {
            var param = Expression.Parameter(typeof(T), "m");
            var body = GetExpressionBody(param, group);
            var expression = Expression.Lambda<Func<T, bool>>(body, param);
            return expression;
        }

        /// <summary>
        ///     获取指定查询条件的查询表达式
        /// </summary>
        /// <typeparam name="T">表达式实体类型</typeparam>
        /// <param name="rule">查询条件，如果为null，则直接返回 true 表达式</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetExpression<T>([CanBeNull] FilterRule rule = null)
        {
            var param = Expression.Parameter(typeof(T), "m");
            var body = GetExpressionBody(param, rule);
            var expression = Expression.Lambda<Func<T, bool>>(body, param);
            return expression;
        }

        /// <summary>
        ///     把查询操作的枚举表示转换为操作码
        /// </summary>
        /// <param name="operation">查询操作的枚举表示</param>
        public static string ToOperateCode(this FilterOperation operation)
        {
            var type = operation.GetType();
            var members = type.GetMember(operation.CastTo<string>());
            if (members.Length > 0)
            {
                var attributes = members[0].GetCustomAttributes(typeof(OperationCodeAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((OperationCodeAttribute)attributes[0]).Code;
                }
            }

            return null;
        }

        /// <summary>
        ///     把查询操作的枚举表示转换为操作名称
        /// </summary>
        /// <param name="operation">查询操作的枚举表示</param>
        /// <returns></returns>
        public static string ToOperateName(this FilterOperation operation)
        {
            var type = operation.GetType();
            var members = type.GetMember(operation.CastTo<string>());
            if (members.Length > 0)
            {
                var attributes = members[0].GetCustomAttributes(typeof(OperationCodeAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((OperationCodeAttribute)attributes[0]).Name;
                }
            }

            return null;
        }

        /// <summary>
        ///     获取操作码的查询操作枚举表示
        /// </summary>
        /// <param name="code">操作码</param>
        /// <returns></returns>
        public static FilterOperation GetFilterOperate(string code)
        {
            var type = typeof(FilterOperation);
            var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var member in members)
            {
                var operate = member.Name.Cast<FilterOperation>();
                if (operate.ToString() == code)
                {
                    return operate.First();
                }
            }


            throw new NotSupportedException("获取操作码的查询操作枚举表示时不支持代码：" + code);
        }

        // /// <summary>
        // /// 获取指定查询条件组的查询表达式，并综合数据权限
        // /// </summary>
        // /// <typeparam name="T">实体类型</typeparam>
        // /// <param name="group">传入的查询条件组，为空时则只返回数据权限过滤器</param>
        // /// <param name="operation">数据权限操作</param>
        // /// <returns>综合之后的表达式</returns>
        // public static Expression<Func<T, bool>> GetDataFilterExpression<T>(FilterGroup group = null,
        //     DataOperation operation = DataOperation.Read)
        // {
        //     var body = Expression.Constant(true, typeof(T));
        //     var para = Expression.Parameter(typeof(T), "m");
        //     var exp = Expression.Lambda<Func<T, bool>>(body, para);
        //     if (group != null)
        //     {
        //         exp = GetExpression<T>(group);
        //     }
        //
        //     //从缓存中查找当前用户的角色与实体T的过滤条件
        //     ClaimsPrincipal user = ServiceLocator.Instance.GetCurrentUser();
        //     if (user == null)
        //     {
        //         return exp;
        //     }
        //
        //     IDataAuthCache dataAuthCache = ServiceLocator.Instance.GetService<IDataAuthCache>();
        //     if (dataAuthCache == null)
        //     {
        //         return exp;
        //     }
        //
        //     string[] roleNames = null;
        //     // 要判断数据权限功能,先要排除没有执行当前功能权限的角色,判断剩余角色的数据权限
        //     if (user.Identity is ClaimsIdentity claimsIdentity)
        //     {
        //         roleNames = claimsIdentity.FindAll(ClaimTypes.Role).SelectMany(m =>
        //         {
        //             var roles = m.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //             return roles;
        //         }).ToArray();
        //     }
        //     else
        //     {
        //         roleNames = Array.Empty<string>();
        //     }
        //
        //
        //     ScopedDictionary scopedDict = ServiceLocator.Instance.GetService<ScopedDictionary>();
        //     if (scopedDict?.Function != null)
        //     {
        //         roleNames = scopedDict.DataAuthValidRoleNames;
        //     }
        //
        //     string typeName = typeof(T).GetFullNameWithModule();
        //     Expression<Func<T, bool>> subExp = null;
        //     foreach (string roleName in roleNames)
        //     {
        //         FilterGroup subGroup = dataAuthCache.GetFilterGroup(roleName, typeName, operation);
        //         if (subGroup == null)
        //         {
        //             continue;
        //         }
        //
        //         // 各个角色的数据过滤条件使用Or连接
        //         subExp = subExp == null ? GetExpression<T>(subGroup) : subExp.Or(GetExpression<T>(subGroup));
        //     }
        //
        //     if (subExp != null)
        //     {
        //         if (group == null)
        //         {
        //             return subExp;
        //         }
        //
        //         // 数据权限条件与主条件使用And连接
        //         exp = subExp.And(exp);
        //     }
        //
        //     return exp;
        // }

        private static Expression GetExpressionBody(ParameterExpression param, FilterGroup group)
        {
            Check.NotNull(param, nameof(param));

            //如果无条件或条件为空，直接返回 true表达式
            if (group == null || (group.Rules.Count == 0 && group.Groups.Count == 0))
            {
                return Expression.Constant(true);
            }

            var bodys = new List<Expression>();
            bodys.AddRange(group.Rules.Select(rule => GetExpressionBody(param, rule)));
            bodys.AddRange(group.Groups.Select(subGroup => GetExpressionBody(param, subGroup)));

            if (group.Operation == FilterOperation.And)
            {
                return bodys.Aggregate(Expression.AndAlso);
            }

            if (group.Operation == FilterOperation.Or)
            {
                return bodys.Aggregate(Expression.OrElse);
            }

            throw new InvalidOperationException("The operation type in the query condition group is wrong, and it can only be And or Or.");
        }

        private static Expression GetExpressionBody(ParameterExpression param, FilterRule rule)
        {
            if (rule == null || rule.Value == null || string.IsNullOrEmpty(rule.Value.ToString()))
            {
                return Expression.Constant(true);
            }

            var expression = GetPropertyLambdaExpression(param, rule);
            var constant = ChangeTypeToExpression(rule, expression.Body.Type);
            return ExpressionDict[rule.Operation](expression.Body, constant);
        }

        private static LambdaExpression GetPropertyLambdaExpression(ParameterExpression param, FilterRule rule)
        {
            var propertyNames = rule.Field.Split('.');
            Expression propertyAccess = param;
            var type = param.Type;
            foreach (var propertyName in propertyNames)
            {
                var property = type.GetProperty(propertyName);
                if (property == null)
                {
                    throw new InvalidOperationException(string.Format("The specified property {0} does not exist in the type {1}.", rule.Field,
                        type.FullName));
                }

                type = property.PropertyType;
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }

            return Expression.Lambda(propertyAccess, param);
        }

        private static Expression ChangeTypeToExpression(FilterRule rule, Type conversionType)
        {
            //if (item.Method == QueryMethod.StdIn)
            //{
            //    Array array = (item.Value as Array);
            //    List<Expression> expressionList = new List<Expression>();
            //    if (array != null)
            //    {
            //        expressionList.AddRange(array.Cast<object>().Select((t, i) =>
            //            ChangeType(array.GetValue(i), conversionType)).Select(newValue => Expression.Constant(newValue, conversionType)));
            //    }
            //    return Expression.NewArrayInit(conversionType, expressionList);
            //}

            var elementType = conversionType.GetUnNullableType();
            var value = rule.Value is string
                ? rule.Value.ToString().CastTo(conversionType)
                : Convert.ChangeType(rule.Value, elementType);
            return Expression.Constant(value, conversionType);
        }
    }
}