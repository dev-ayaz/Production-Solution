// ┌────────────────────────────────────────────────────────────────────┐ \\
// │ IView By dev_Ayaz                                                  │ \\
// ├────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2011-2017 Muhammad Ayaz (dev_ayaz@yahoo.com)           │ \\
// │ Copyright © 2011-2017 Muhammad Ayaz (dev_ayaz@yahoo.com)           │ \\
// ├────────────────────────────────────────────────────────────────────┤ \\
// │                                                                    │ \\
// └────────────────────────────────────────────────────────────────────┘ \\

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Production.DataAccess.Contracts
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(int pageNumber = 0, int pageSize = 0,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        IQueryable<T> GetAll();

        IQueryable<T> GetWhere(Expression<Func<T, bool>> filter = null, string includeProperties = "");

        T GetById(long entityId);

        T Insert(T entity);

        IEnumerable<T> InsertRange(List<T> entity);

        void Update(T entity);

        void Upsert(T entity);

        void Delete(T entity);

        void Delete(long entityId);

        int WhereCount(Expression<Func<T, bool>> filter);

        int Count();

        void DetachEntity(T entity);
    }
}
