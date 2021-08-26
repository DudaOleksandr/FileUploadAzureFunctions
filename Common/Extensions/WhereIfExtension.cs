using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Extensions
{
    public static class WhereIfExtension
    {
        public static IEnumerable<TCollection> WhereIf<TCollection>(this IEnumerable<TCollection> source,
            bool condition, Func<TCollection, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        public static IQueryable<TCollection> WhereIf<TCollection>(this IQueryable<TCollection> source, bool condition,
            Expression<Func<TCollection, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}
