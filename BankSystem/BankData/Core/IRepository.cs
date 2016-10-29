using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BankData.Core
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        void Create(T entity);
        void Update(T entity);
        void Remove(int id);
        void Remove(T entity);
    }
}
