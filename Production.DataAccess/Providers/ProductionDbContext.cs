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
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Production.DataAccess.Providers
{
    public class ProductionDbContext : DbContext
    {
        public bool EnforceConsistency
        {
            get
            {
                var isEnabled = ConfigurationManager.AppSettings["EnforceDbConsistency"];

                return isEnabled != null && Convert.ToBoolean(isEnabled);
            }
        }

        public ProductionDbContext() : base("name=ProductionDbContext")
        {
        }

        public ProductionDbContext(string dbConnectionString) : base(dbConnectionString)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;

            Database.CommandTimeout = 300000;

            if (!EnforceConsistency)
            {
                Database.SetInitializer<ProductionDbContext>(null);
            }
        }


        public static ProductionDbContext Create()
        {
            return new ProductionDbContext();
        }



    }
}
