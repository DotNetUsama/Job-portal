using System;
using System.Linq;

namespace Job_Portal_System.Data
{
    public static class IQueryableExtensions
    {
        private static readonly Random Randomizer = new Random();

        public static T Random<T>(this IQueryable<T> queryable, int queryableCount)
        {
            return queryable.Skip(Randomizer.Next(0, queryableCount)).Take(1).First();
        }
    }
}
