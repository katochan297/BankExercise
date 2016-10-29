using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BankData.Core;

namespace BankData.Repository
{
    internal class Repository<T> : IRepository<T> where T : class
    {
        protected static readonly object obj_lock = new object();
        protected readonly DbContext Context;
        private readonly DbSet<T> _dbSet;
        internal Repository(DbContext context)
        {
            Context = context;
            _dbSet = Context.Set<T>();
        }
        
        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.SingleOrDefault(predicate);
        }

        public void Create(T entity)
        {
            _dbSet.Attach(entity);
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(int id)
        {
            T entityTodelete = GetById(id);
            Remove(entityTodelete);
        }
        public void Remove(T entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }


    }
}
