using Collate.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Collate
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filter an IQueryable by one or more fields
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="source">The collection to be filtered</param>
        /// <param name="request">An IFilterRequest with one or more Filter objects by which to filter the collection</param>
        /// <returns>The IQueryable, filtered to include only items which match the specified filters</returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, IFilterRequest request)
        {
            if (request.Filters != null && request.Filters.Any())
            {
                var expression = ExpressionBuilder.GetExpression<T>(request.Logic, request.Filters);
                return source.Where(expression);
            }

            return source;
        }

        /// <summary>
        /// Filter an IQueryable by one or more fields
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="source">The collection to be filtered</param>
        /// <param name="filters">One or more Filter objects by which to filter the collection</param>
        /// <returns>The IQueryable, filtered to include only items which match the specified fitlers</returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, IEnumerable<IFilter> filters)
        {
            if (filters != null && filters.Any())
            {
                var expression = ExpressionBuilder.GetExpression<T>(FilterLogic.And, filters);
                return source.Where(expression);
            }

            return source;
        }

        /// <summary>
        /// Sort an IQueryable by one or more fields
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="source">The collection to be sorted</param>
        /// <param name="request">An ISortRequest with one or more Sort objects specifying a field and direction by which to order the collection</param>
        /// <returns>The IQueryable, sorted as specified by the ISortRequest</returns>
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

        /// <summary>
        /// Sort an IQueryable by a value on a navigation property of the main collection (e.g. sort Album by Artist.Name)
        /// </summary>
        /// <typeparam name="T">The type of the base collection (e.g. Album)</typeparam>
        /// <param name="query">The base collection to sort (e.g. IQueryable&lt;Album&gt;)</param>
        /// <param name="sort">The sort to be applied (e.g. Field = 'Name', Direction = 'Ascending')</param>
        /// <param name="navigationPropertyName">The navigation property to sort on (e.g. 'Artist')</param>
        /// <returns>The IQueryable, sorted by the navigation property specified</returns>
        public static IQueryable<T> NavigationSort<T>(this IQueryable<T> query, ISort sort, string navigationPropertyName)
        {
            var param = Expression.Parameter(typeof(T), "p");
            Expression parent = param;
            parent = Expression.Property(parent, navigationPropertyName);
            parent = Expression.Property(parent, sort.Field);

            var sortExpression = Expression.Lambda<Func<T, object>>(parent, param);

            if (sort.Direction == SortDirection.Ascending)
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(sortExpression);
            }

            return query;
        }

        /// <summary>
        /// Get a single page of results from an IOrderedQueryable.
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="source">The collection to be paged</param>
        /// <param name="request">An IPageRequest specifying a PageSize and a PageNumber to reduce the collection to</param>
        /// <returns>The IQueryable with Skip() and Take() applied</returns>
        public static IQueryable<T> Page<T>(this IOrderedQueryable<T> source, IPageRequest request)
        {
            return source
                .Skip(request.GetSkip())
                .Take(request.PageSize);
        }

        /// <summary>
        /// Get a single page of results from an IQueryable.
        /// </summary>
        /// <typeparam name="T">The collection type</typeparam>
        /// <param name="source">The collection to be paged</param>
        /// <param name="request">An IPageRequest specifying a PageSize and a PageNumber to by which to reduce the collection</param>
        /// <returns>The IQueryable with Skip() and Take() applied</returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> source, IPageRequest request)
        {
            return source
                .Skip(request.GetSkip())
                .Take(request.PageSize);
        }

        /// <summary>
        /// Translates the PageNumber and PageSize into a "skip" value for use in .Skip().Take() LINQ methods.
        /// </summary>
        /// <param name="request">The page request</param>
        /// <returns>An integer calculated from the PageNumber and PageSize of the page request (e.g. PageSize = 25 and PageNumber = 3 results in a Skip of 50)</returns>
        private static int GetSkip(this IPageRequest request)
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;

            return (pageNumber - 1) * request.PageSize;
        }
    }
}
