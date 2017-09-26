// ┌────────────────────────────────────────────────────────────────────┐ \\
// │ Maintenance Db Context By dev_Ayaz                                 │ \\
// ├────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2011-2017 Muhammad Ayaz (dev_ayaz@yahoo.com)           │ \\
// │ Copyright © 2011-2017 Muhammad Ayaz (dev_ayaz@yahoo.com)           │ \\
// ├────────────────────────────────────────────────────────────────────┤ \\
// │                                                                    │ \\
// └────────────────────────────────────────────────────────────────────┘ \\

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Production.DataAccess.Contracts;

namespace Production.DataAccess.Providers
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public BaseRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = DbContext.Set<T>();
        }

        public IQueryable<T> GetAll(int pageNumber = 0, int pageSize = 0,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = DbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageNumber == 0 || pageSize == 0)
                return query;

            var rowsCount = query.Count();

            if (rowsCount <= pageSize)
                pageNumber = 1;

            //Calculate nunber of rows to skip on pagesize
            var excludedRows = (pageNumber - 1) * pageSize;

            //Skip the required rows for the current page and take the next records of pagesize count
            return query.Skip(excludedRows).Take(pageSize);
        }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public virtual T GetById(long id)
        {
            //return DbSet.FirstOrDefault(PredicateBuilder.GetByIdPredicate<T>(id));
            return DbSet.Find(id);
        }

        public virtual T Insert(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                return DbSet.Add(entity);
            }
            return null;
        }

        public virtual IEnumerable<T> InsertRange(List<T> entity)
        {
            return DbSet.AddRange(entity);
        }

        public virtual void Upsert(T entity)
        {
            DbContext.Set<T>().AddOrUpdate(entity);
        }

        public virtual void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual void Delete(long id)
        {
            var entity = GetById(id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
        }

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> filter = null,
            string includeProperties = "")
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return query;
        }

        public int WhereCount(Expression<Func<T, bool>> filter)
        {
            return filter != null ? DbSet.Count(filter) : DbSet.Count();
        }

        public int Count()
        {
            return DbSet.Count();
        }

        public virtual void DetachEntity(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Detached;
        }
    }
}
