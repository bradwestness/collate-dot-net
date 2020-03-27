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
            if (!(sorts is object) || !sorts.Any())
            {
                return;
            }

            var isOrdered = source.Expression.Type.Equals(typeof(IOrderedQueryable<T>));
            var sortList = sorts.ToList();
            var itemType = typeof(T);
            var parameter = Expression.Parameter(itemType, "item");

            foreach (var sort in sortList)
            {
                var property = typeof(T).GetProperty(sort.Field);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var sortExpression = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression methodCall = null;

                switch (sort.Direction)
                {
                    case SortDirection.Ascending:
                        methodCall = !isOrdered
                            ? Expression.Call(typeof(Queryable), nameof(Queryable.OrderBy), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression))
                            : Expression.Call(typeof(Queryable), nameof(Queryable.ThenBy), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression));
                        break;

                    case SortDirection.Descending:
                        methodCall = !isOrdered
                            ? Expression.Call(typeof(Queryable), nameof(Queryable.OrderByDescending), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression))
                            : Expression.Call(typeof(Queryable), nameof(Queryable.ThenByDescending), new Type[] { itemType, property.PropertyType }, source.Expression, Expression.Quote(sortExpression));
                        break;

                    default:
                        throw new NotImplementedException($"Unsupported sort direction: {sort.Direction}");
                }

                source = source.Provider.CreateQuery<T>(methodCall) as IOrderedQueryable<T>;
                isOrdered = true;
            }
        }

        public static Expression<Func<T, object>> GetSortExpression<T>(ref IOrderedQueryable<T> source, ISort sort, string navigationPropertyName)
        {
            Expression<Func<T, object>> sortExpression = null;

            if (sort is object && navigationPropertyName is object)
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
