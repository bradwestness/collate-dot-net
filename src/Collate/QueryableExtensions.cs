using Collate.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Collate
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, IFilterRequest request)
        {
            if (request.Filters != null && request.Filters.Any())
            {
                var expression = ExpressionBuilder.GetExpression<T>(request.Filters);
                return source.Where(expression);
            }

            return source;
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> source, IEnumerable<IFilter> filters)
        {
            if (filters != null && filters.Any())
            {
                var expression = ExpressionBuilder.GetExpression<T>(filters);
                return source.Where(expression);
            }

            return source;
        }

        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> source, ISortRequest request)
        {
            var dest = source as IOrderedQueryable<T>;

            if (request.Sorts == null || !request.Sorts.Any())
            {
                return dest;
            }

            var sortingList = request.Sorts.ToList();
            var itemType = typeof(T);
            var parameter = Expression.Parameter(itemType, "item");

            for (var i = 0; i < sortingList.Count; i++)
            {
                var property = typeof(T).GetProperty(sortingList[i].Field);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var sortExpression = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression result = null;

                if (i == 0)
                {
                    switch (sortingList[i].Direction)
                    {
                        case SortDirection.Ascending:
                            result = Expression.Call(typeof(Queryable), "OrderBy", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression));
                            break;

                        case SortDirection.Descending:
                            result = Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression));
                            break;
                    }
                }
                else
                {
                    switch (sortingList[i].Direction)
                    {
                        case SortDirection.Ascending:
                            result = Expression.Call(typeof(Queryable), "ThenBy", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression));
                            break;

                        case SortDirection.Descending:
                            result = Expression.Call(typeof(Queryable), "ThenByDescending", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression));
                            break;
                    }
                }

                dest = dest.Provider.CreateQuery<T>(result) as IOrderedQueryable<T>;
            }

            return dest;
        }


        public static IQueryable<T> NestedSort<T>(this IQueryable<T> query, ISort sorting, string nestedObjectName)
        {
            var param = Expression.Parameter(typeof(T), "p");
            Expression parent = param;
            parent = Expression.Property(parent, nestedObjectName);
            parent = Expression.Property(parent, sorting.Field);

            var sortExpression = Expression.Lambda<Func<T, object>>(parent, param);

            if (sorting.Direction == SortDirection.Ascending)
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(sortExpression);
            }

            return query;
        }

        public static IQueryable<T> Page<T>(this IOrderedQueryable<T> source, IPageRequest request)
        {
            return source
                .Skip(request.GetSkip())
                .Take(request.PageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> source, IPageRequest request)
        {
            return source
                .Skip(request.GetSkip())
                .Take(request.PageSize);
        }

        private static int GetSkip(this IPageRequest request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;

            return (pageNumber - 1) * request.PageSize;
        }
    }
}
