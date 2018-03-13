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
        /// Filters an IQueryable by one or more fields.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be filtered.</param>
        /// <param name="request">An IFilterRequest with one or more Filter objects by which to filter the collection.</param>
        /// <returns>the IQueryable, filtered to include only items which match the specified filter(s).</returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, IFilterRequest request)
        {
            if (request != null && request.Filters != null && request.Filters.Any())
            {
                var expression = ExpressionBuilder.GetExpression<T>(request.Logic, request.Filters);
                return source.Where(expression);
            }

            return source;
        }

        /// <summary>
        /// Filters an IQueryable by one or more fields.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be filtered.</param>
        /// <param name="filters">One or more Filter objects by which to filter the collection.</param>
        /// <returns>the IQueryable, filtered to include only items which match the specified fitler(s).</returns>
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
        /// Sorts an IQueryable by one or more fields.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be sorted.</param>
        /// <param name="request">An ISortRequest with one or more Sort objects specifying a field and direction by which to order the collection.</param>
        /// <returns>the IQueryable, sorted as specified by the ISortRequest.</returns>
        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> source, ISortRequest request)
        {
            if (request != null && request.Sorts != null)
            {
                return source.Sort(request.Sorts);
            }

            return source as IOrderedQueryable<T>;
        }

        /// <summary>
        /// Sorts an IQueryable by one or more fields.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be sorted.</param>
        /// <param name="sorts">One or more ISort objects by which to sort the collection..</param>
        /// <returns>the IOrderedQueryable, sorted as specified by the ISort object(s).</returns>
        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> source, IEnumerable<ISort> sorts)
        {
            var dest = source as IOrderedQueryable<T>;

            if (sorts == null || !sorts.Any())
            {
                return dest;
            }

            var sortList = sorts.ToList();
            var itemType = typeof(T);
            var parameter = Expression.Parameter(itemType, "item");

            for (var i = 0; i < sortList.Count; i++)
            {
                var property = typeof(T).GetProperty(sortList[i].Field);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var sortExpression = Expression.Lambda(propertyAccess, parameter);
                MethodCallExpression result = null;

                if (i == 0)
                {
                    result = (sortList[i].Direction == SortDirection.Ascending)
                        ? Expression.Call(typeof(Queryable), "OrderBy", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression))
                        : Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression));
                }
                else
                {
                    result = (sortList[i].Direction == SortDirection.Ascending)
                        ? Expression.Call(typeof(Queryable), "ThenBy", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression))
                        : Expression.Call(typeof(Queryable), "ThenByDescending", new Type[] { itemType, property.PropertyType }, dest.Expression, Expression.Quote(sortExpression));
                }

                dest = dest.Provider.CreateQuery<T>(result) as IOrderedQueryable<T>;
            }

            return dest;
        }

        /// <summary>
        /// Sorts an IQueryable by a value on a navigation property of the main collection (e.g. sort Album by Album.Artist.Name).
        /// </summary>
        /// <typeparam name="T">The type of the base collection (e.g. Album).</typeparam>
        /// <param name="source">The base collection to sort (e.g. IQueryable&lt;Album&gt;).</param>
        /// <param name="sort">The sort to be applied (e.g. Field = 'Name', Direction = 'Ascending').</param>
        /// <param name="navigationPropertyName">The navigation property to sort on (e.g. 'Artist').</param>
        /// <returns>the IOrderedQueryable, sorted by the navigation property specified.</returns>
        public static IOrderedQueryable<T> NavigationSort<T>(this IQueryable<T> source, ISort sort, string navigationPropertyName)
        {
            var dest = source as IOrderedQueryable<T>;

            if (sort != null && navigationPropertyName != null)
            {
                var param = Expression.Parameter(typeof(T), "p");
                Expression parent = param;
                parent = Expression.Property(parent, navigationPropertyName);
                parent = Expression.Property(parent, sort.Field);

                var sortExpression = Expression.Lambda<Func<T, object>>(parent, param);

                dest = (sort.Direction == SortDirection.Ascending)
                    ? source.OrderBy(sortExpression)
                    : source.OrderByDescending(sortExpression);
            }

            return dest;
        }

        /// <summary>
        /// Returns a single page of results from an IOrderedQueryable.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be paged.</param>
        /// <param name="request">An IPageRequest specifying a PageSize and a PageNumber to reduce the collection by.</param>
        /// <returns>the IOrderedQueryable with Skip() and Take() applied.</returns>
        public static IQueryable<T> Page<T>(this IOrderedQueryable<T> source, IPageRequest request)
        {
            if (request == null)
            {
                return source;
            }

            return source.Page(request.PageNumber, request.PageSize);
        }

        /// <summary>
        /// Returns a single page of results from an IQueryable.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be paged.</param>
        /// <param name="request">An IPageRequest specifying a PageSize and a PageNumber to by which to reduce the collection.</param>
        /// <returns>the IQueryable with Skip() and Take() applied.</returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> source, IPageRequest request)
        {
            if (request == null)
            {
                return source;
            }

            return source.Page(request.PageNumber, request.PageSize);
        }

        /// <summary>
        /// Returns a single page of results from an IQueryable.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be paged.</param>
        /// <param name="request">An IPageRequest specifying a PageSize and a PageNumber to by which to reduce the collection.</param>
        /// <returns>the IQueryable with Skip() and Take() applied.</returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            return source
                .Skip(GetSkip(pageNumber, pageSize))
                .Take(pageSize);
        }

        /// <summary>
        /// Translates PageNumber and PageSize into a skip value to be used in the .Skip() LINQ method.
        /// </summary>
        /// <param name="pageNumber">The one-based page of results to be returned (anything less than 1 will be reset to 1).</param>
        /// <param name="pageSize">The number of results to return.</param>
        /// <returns>the calculated skip value for use in the .Skip() LINQ method.</returns>
        private static int GetSkip(int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            return (pageNumber - 1) * pageSize;
        }
    }
}
