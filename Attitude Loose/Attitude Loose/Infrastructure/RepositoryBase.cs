using Attitude_Loose.App_Start;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Attitude_Loose.Infrastructure
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        private EntitiesInitial dataContext;
        private readonly IDbSet<T> dbset;
        private IDatabaseFactory databaseFactory;
        protected RepositoryBase(IDatabaseFactory databaseFactory)
        {
            this.databaseFactory = databaseFactory;
            dbset = DataContext.Set<T>();
        }

        protected EntitiesInitial DataContext
        {
            get { return dataContext ?? (dataContext = databaseFactory.Get()); }
        }

        public virtual void Add(T entity)
        {
            dbset.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).FirstOrDefault<T>();
        }

        public virtual void Update(T entity)
        {
            dataContext.Entry(entity).CurrentValues.SetValues(entity);
            //dbset.Attach(entity);
            //dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return dbset.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbset.Where(where).ToList();
        }
    }
}