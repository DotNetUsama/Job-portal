using System;
using System.Linq;

namespace Job_Portal_System.Data
{
    public static class QueryableExtensions
    {
        private static readonly Random Randomize = new Random();

        public static T Random<T>(this IQueryable<T> queryable, int queryableCount)
        {
            return queryable.Skip(Randomize.Next(0, queryableCount)).Take(1).First();
        }
    }
}
