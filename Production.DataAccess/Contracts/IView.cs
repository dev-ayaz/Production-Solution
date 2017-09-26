// ┌────────────────────────────────────────────────────────────────────┐ \\
// │ IView By dev_Ayaz                                                  │ \\
// ├────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2011-2017 Muhammad Ayaz (dev_ayaz@yahoo.com)           │ \\
// │ Copyright © 2011-2017 Muhammad Ayaz (dev_ayaz@yahoo.com)           │ \\
// ├────────────────────────────────────────────────────────────────────┤ \\
// │                                                                    │ \\
// └────────────────────────────────────────────────────────────────────┘ \\

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Production.DataAccess.Contracts
{
    public interface IView<T> where T : class
    {
        IQueryable<T> Retrieve(int pageNumber = 0, int pageSize = 0,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        IQueryable<T> Retrieve();
    }
}
