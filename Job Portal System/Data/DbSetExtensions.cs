using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Data
{
    public static class DbSetExtensions
    {
        public static T FindOrAdd<T>(this DbSet<T> dbSet, 
            T entity, Expression<Func<T, bool>> predicate = null) where T : class, new()
        {
            return predicate != null ? dbSet.SingleOrDefault(predicate) ?? dbSet.Add(entity).Entity : null;
        }
    }
}
