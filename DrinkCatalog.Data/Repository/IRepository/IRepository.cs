using System.Linq.Expressions;

namespace DrinkCatalog.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T GetById(Expression<Func<T, bool>> filter, string? includeProperties = null);
        IEnumerable<T> Find (Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
    }
}
