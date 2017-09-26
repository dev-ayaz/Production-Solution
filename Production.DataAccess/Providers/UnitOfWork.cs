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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Production.DataAccess.Contracts;
using Production.DataAccess.Providers.Helpers;

namespace Production.DataAccess.Providers
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        //************** GEO Schema Entities **************************************
        //public IRepository<Region> Regions => GetStandardRepository<Region>();

        public UnitOfWork(IRepositoryProvider repositoryProvider) : this(repositoryProvider, "MaintenanceDbContext") { }

        public UnitOfWork(IRepositoryProvider repositoryProvider, string dbConnectionString) : base(repositoryProvider, dbConnectionString)
        {
        }

        protected override void CreateDbContext(string dbConnectionString)
        {
            DbContext = string.IsNullOrEmpty(dbConnectionString) ? new MaintenanceDbContext() : new MaintenanceDbContext(dbConnectionString);
        }
    }
}
