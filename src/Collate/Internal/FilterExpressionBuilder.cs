using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Collate.Internal
{
    internal static class FilterExpressionBuilder
    {
        private static readonly MethodInfo _containsMethod = typeof(string).GetMethod(nameof(FilterOperator.Contains), new[] { typeof(string) });
        private static readonly MethodInfo _startsWithMethod = typeof(string).GetMethod(nameof(FilterOperator.StartsWith), new[] { typeof(string) });
        private static readonly MethodInfo _endsWithMethod = typeof(string).GetMethod(nameof(FilterOperator.EndsWith), new[] { typeof(string) });

        public static Expression<Func<T, bool>> GetFilterExpression<T>(FilterLogic filterLogic, IEnumerable<IFilter> filters, Expression<Func<T, bool>> additionalExpression = null)
        {
            if (filters == null || !filters.Any())
            {
                return (additionalExpression != null)
                    ? additionalExpression
                    : x => true;
            }

            var filterList = filters.ToList();
            ParameterExpression param = Expression.Parameter(typeof(T), "item");
            Expression expression = additionalExpression;

            for (int i = 0; i < filterList.Count; i++)
            {
                var item = filterList[i];
                var property = typeof(T).GetProperty(item.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                if (property != null && !string.IsNullOrEmpty(item.Value))
                {
                    if (expression == null)
                    {
                        expression = GetFilterExpression<T>(item.Operator, param, item.Value, item.Field);
                    }
                    else
                    {
                        switch (filterLogic)
                        {
                            case FilterLogic.And:
                                expression = Expression.AndAlso(expression, GetFilterExpression<T>(item.Operator, param, item.Value, item.Field));
                                break;

                            case FilterLogic.Or:
                                expression = Expression.OrElse(expression, GetFilterExpression<T>(item.Operator, param, item.Value, item.Field));
                                break;

                            default:
                                throw new ArgumentException($"Invalid filter logic: {filterLogic}.", nameof(filterLogic));
                        }
                    }
                }
            }

            if (expression == null)
            {
                return x => true;
            }

            return Expression.Lambda<Func<T, bool>>(expression, param);
        }

        private static Expression GetFilterExpression<T>(FilterOperator filterOperator, ParameterExpression param, string filterValue, string fieldName)
        {
            MemberExpression member = Expression.Property(param, fieldName);
            ConstantExpression constant;

            if (member.Type == typeof(bool) || member.Type == typeof(bool?))
            {
                constant = Expression.Constant(bool.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(byte) || member.Type == typeof(byte?))
            {
                constant = Expression.Constant(byte.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(char) || member.Type == typeof(char?))
            {
                constant = Expression.Constant(char.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(DateTime) || member.Type == typeof(DateTime?))
            {
                constant = Expression.Constant(DateTime.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(DateTimeOffset) || member.Type == typeof(DateTimeOffset?))
            {
                constant = Expression.Constant(DateTimeOffset.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(decimal) || member.Type == typeof(decimal?))
            {
                constant = Expression.Constant(decimal.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(double) || member.Type == typeof(double?))
            {
                constant = Expression.Constant(double.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(float) || member.Type == typeof(float?))
            {
                constant = Expression.Constant(float.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(Guid) || member.Type == typeof(Guid?))
            {
                constant = Expression.Constant(Guid.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(int) || member.Type == typeof(int?))
            {
                constant = Expression.Constant(int.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(long) || member.Type == typeof(long?))
            {
                constant = Expression.Constant(long.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(sbyte) || member.Type == typeof(sbyte?))
            {
                constant = Expression.Constant(sbyte.Parse(filterValue), member.Type);
            }
            else if (member.Type == typeof(short) || member.Type == typeof(short?))
            {
                constant = Expression.Constant(short.Parse(filterValue), member.Type);
            }
            else
            {
                constant = Expression.Constant(filterValue, member.Type);
            }

            Expression expression;

            switch (filterOperator)
            {
                case FilterOperator.GreaterThanOrEqual:
                    expression = Expression.GreaterThanOrEqual(member, constant);
                    break;

                case FilterOperator.LessThanOrEqual:
                    expression = Expression.LessThanOrEqual(member, constant);
                    break;

                case FilterOperator.LessThan:
                    expression = Expression.LessThan(member, constant);
                    break;

                case FilterOperator.GreaterThan:
                    expression = Expression.GreaterThan(member, constant);
                    break;

                case FilterOperator.Equal:
                    expression = Expression.Equal(member, constant);
                    break;

                case FilterOperator.NotEqual:
                    expression = Expression.NotEqual(member, constant);
                    break;

                case FilterOperator.Contains:
                    expression = Expression.Call(member, _containsMethod, constant);
                    break;

                case FilterOperator.DoesNotContain:
                    expression = Expression.Not(Expression.Call(member, _containsMethod, constant));
                    break;

                case FilterOperator.EndsWith:
                    expression = Expression.Call(member, _endsWithMethod, constant);
                    break;

                case FilterOperator.StartsWith:
                    expression = Expression.Call(member, _startsWithMethod, constant);
                    break;

                default:
                    throw new ArgumentException($"Invalid filter operator: {filterOperator}.", nameof(filterOperator));
            }

            return expression;
        }
    }
}
