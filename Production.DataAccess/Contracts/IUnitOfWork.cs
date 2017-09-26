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
using System.Text;
using System.Threading.Tasks;

namespace Production.DataAccess.Contracts
{
    public interface IUnitOfWork : IBaseUnitOfWork, IDisposable
    {
        //************** GEO Schema Entities **************************************
        //IRepository<Region> Regions { get; }
    }
}
