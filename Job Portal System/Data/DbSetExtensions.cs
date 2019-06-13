using Microsoft.EntityFrameworkCore;

namespace Job_Portal_System.Data
{
    public static class DbSetExtensions
    {
        public static void RemoveAll<T>(this DbSet<T> dbSet) where T : class, new()
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
