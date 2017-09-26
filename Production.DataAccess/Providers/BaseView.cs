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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Production.DataAccess.Contracts;

namespace Production.DataAccess.Providers
{
    class BaseView<T> : IView<T> where T : class
    {
        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public BaseView(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = DbContext.Set<T>();
        }

        public IQueryable<T> Retrieve(int pageNumber = 0, int pageSize = 0,
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

        public virtual IQueryable<T> Retrieve()
        {
            return DbSet;
        }
    }
}
