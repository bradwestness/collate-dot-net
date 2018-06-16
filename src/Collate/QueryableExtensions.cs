using Collate.Implementation;
using Collate.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collate
{
    public static class Extensions
    {
        private const char DELIMITER = ',';
        private const char NEGATOR = '-';

        /// <summary>
        /// Converts a string of comma-separated field names into a collection of ISort objects.
        /// Fields prefixed with a minus sign (e.g. "-FirstName") will be sorted in Descending order,
        /// otherwise fields are sorted in Ascending order by default.
        /// </summary>
        /// <param name="sortString">A comma-separated list of field names.</param>
        /// <returns>A collection of ISort objects.</returns>
        public static IEnumerable<ISort> ToSorts(this string sortString)
        {
            if (string.IsNullOrEmpty(sortString))
            {
                return Array.Empty<ISort>();
            }

            var list = new List<ISort>();

            foreach (var field in sortString.Split(new[] { DELIMITER }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (field.IndexOf(NEGATOR) == 0)
                {
                    list.Add(new Sort
                    {
                        Field = field.TrimStart(new[] { NEGATOR }),
                        Direction = SortDirection.Descending
                    });
                }
                else
                {
                    list.Add(new Sort
                    {
                        Field = field,
                        Direction = SortDirection.Ascending
                    });
                }
            }

            return list;
        }

        public static ISortRequest ToSortRequest(this string sortString)
        {
            return new SortRequest
            {
                Sorts = sortString.ToSorts()
            };
        }

        public static IEnumerable<IFilter> ToFilters<T>(this IEnumerable<T> values, string fieldName, FilterOperator filterOperator)
        {
            var list = new List<IFilter>();

            foreach (T value in values)
            {
                list.Add(new Filter
                {
                    Field = fieldName,
                    Operator = filterOperator,
                    Value = value.ToString()
                });
            }

            return list;
        }

        public static IFilterRequest ToFilterRequest<T>(this IEnumerable<T> values, string fieldName, FilterOperator filterOperator, FilterLogic filterLogic)
        {
            return new FilterRequest
            {
                Filters = values.ToFilters(fieldName, filterOperator),
                Logic = filterLogic
            };
        }

        /// <summary>
        /// Filters an IQueryable by a single field.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be filtered.</param>
        /// <param name="filterOperator">The filter operator to use when filtering the collection.</param>
        /// <param name="fieldName">The name of the field to filter by (must exist as a public field on the collection type).</param>
        /// <param name="fieldValue">The value to filter by.</param>
        /// <returns>the IQueryable, filtered to include only items which match the specified filter.</returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, FilterOperator filterOperator, string fieldName, string fieldValue)
        {
            if (fieldName != null)
            {
                var filters = new IFilter[] { new Filter { Operator = filterOperator, Field = fieldName, Value = fieldValue } };
                source.Filter(filters);
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
                var expression = FilterExpressionBuilder.GetFilterExpression<T>(FilterLogic.And, filters);
                return source.Where(expression);
            }

            return source;
        }

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
                var expression = FilterExpressionBuilder.GetFilterExpression<T>(request.Logic, request.Filters);
                return source.Where(expression);
            }

            return source;
        }

        public static IQueryable<T1> Filter<T1, T2>(this IQueryable<T1> source, IEnumerable<T2> values, string fieldName, FilterOperator filterOperator, FilterLogic filterLogic)
        {
            return source.Filter(values.ToFilterRequest(fieldName, filterOperator, filterLogic));
        }

        /// <summary>
        /// Sorts an IQueryable by a single field.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be sorted.</param>
        /// <param name="sortField">The field by which to sort the collection.</param>
        /// <param name="sortDirection">The direction in which to sort the collection.</param>
        /// <returns>the IOrderedQueryable, sorted as specified by the sortField and sortDirection.</returns>
        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> source, string sortField, SortDirection sortDirection)
        {
            if (sortField != null)
            {
                var sorts = new ISort[] { new Sort { Field = sortField, Direction = sortDirection } };
                return source.Sort(sorts);
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

            SortExpressionBuilder.ApplySorts(ref dest, sorts);

            return dest;
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
        /// Sorts an IQueryable by one or more fields with a sort string.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be sorted.</param>
        /// <param name="sortString">A comma-delimited string of fields to sort on. Prefixing a field with a minus sign ('-') will sort by descending order for that field.</param>
        /// <returns>the IQueryable, sorted as specified by the sort string.</returns>
        public static IOrderedQueryable<T> Sort<T>(this IQueryable<T> source, string sortString)
        {
            return source.Sort(sortString.ToSorts());
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
            var sortExpression = SortExpressionBuilder.GetSortExpression(ref dest, sort, navigationPropertyName);

            if (sortExpression != null)
            {
                dest = (sort.Direction == SortDirection.Ascending)
                    ? source.OrderBy(sortExpression)
                    : source.OrderByDescending(sortExpression);
            }

            return dest;
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
        /// Returns a single page of results from an IQueryable.
        /// </summary>
        /// <typeparam name="T">The collection type.</typeparam>
        /// <param name="source">The collection to be paged.</param>
        /// <param name="request">An IPageRequest specifying a PageSize and a PageNumber to by which to reduce the collection.</param>
        /// <returns>the IQueryable with Skip() and Take() applied.</returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> source, IPageRequest request)
        {
            if (request != null)
            {
                return source.Page(request.PageNumber, request.PageSize);
            }

            return source;
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
            if (request != null)
            {
                return source.Page(request.PageNumber, request.PageSize);
            }

            return source;
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
