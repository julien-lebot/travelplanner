using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TravelPlanner.Infrastructure
{
    public interface IRepository<T, in TId> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        T GetById(TId id);
        T Get(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> predicate);

        // For OData mapping
        IQueryable<T> Entities
        {
            get;
        }
    }
}