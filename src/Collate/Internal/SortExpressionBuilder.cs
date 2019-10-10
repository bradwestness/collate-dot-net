using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Collate.Internal
{
    internal static class SortExpressionBuilder
    {
        public static void ApplySorts<T>(ref IOrderedQueryable<T> source, IEnumerable<ISort> sorts)
        {
            if (sorts == null || !sorts.Any())
            {
                return;
            }

            var sortList = sorts.ToList();
            var itemType = typeof(T);
            var parameter = Expression.Parameter(itemType, "item");

            for (var i = 0; i < sortList.Count; i++)
            {
                var property = typeof(T).GetProperty(sortList[i].Field);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var sortExpression = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression methodCall = null;

                if (i == 0)
                {
                    methodCall = (sortList[i].Direction == SortDirection.Ascending)
                        ? Expression.Call(typeof(Queryable), nameof(Queryable.OrderBy), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression))
                        : Expression.Call(typeof(Queryable), nameof(Queryable.OrderByDescending), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression));
                }
                else
                {
                    methodCall = (sortList[i].Direction == SortDirection.Ascending)
                        ? Expression.Call(typeof(Queryable), nameof(Queryable.ThenBy), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression))
                        : Expression.Call(typeof(Queryable), nameof(Queryable.ThenByDescending), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression));
                }

                source = source.Provider.CreateQuery<T>(methodCall) as IOrderedQueryable<T>;
            }
        }

        public static Expression<Func<T, object>> GetSortExpression<T>(ref IOrderedQueryable<T> source, ISort sort, string navigationPropertyName)
        {
            Expression<Func<T, object>> sortExpression = null;

            if (sort != null && navigationPropertyName != null)
            {
                var param = Expression.Parameter(typeof(T), "p");
                Expression parent = param;
                parent = Expression.Property(parent, navigationPropertyName);
                parent = Expression.Property(parent, sort.Field);

                sortExpression = Expression.Lambda<Func<T, object>>(parent, param);
            }

            return sortExpression;
        }
    }
}
