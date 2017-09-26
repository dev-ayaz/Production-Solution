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
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Production.DataAccess.Contracts;
using Production.DataAccess.Providers.Helpers;

namespace Production.DataAccess.Providers
{
    public abstract class BaseUnitOfWork : IBaseUnitOfWork, IDisposable
    {
        protected DbContext DbContext { get; set; }

        protected IRepositoryProvider RepositoryProvider { get; set; }

        public bool EnforceConsistency
        {
            get
            {
                var isEnabled = ConfigurationManager.AppSettings["EnforceDbConsistency"];

                if (isEnabled != null)
                {
                    return Convert.ToBoolean(isEnabled);
                }

                return false;
            }
        }

        protected BaseUnitOfWork(IRepositoryProvider repositoryProvider, string dbConnectionString)
        {
            CreateDbContext(dbConnectionString);
            repositoryProvider.DbContext = DbContext;
            RepositoryProvider = repositoryProvider;

            // Do NOT enable proxied entities, else serialization fails
            DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            DbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need 
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.

            //DbContext.Database.Log = msg => Trace.WriteLine(msg);
        }

        public int Commit()
        {
            return DbContext.SaveChanges();
        }

        public async Task<int> CommitAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public int SetIdnetityInsert<T>(bool state) where T : class
        {
            var tableName = GetTableName<T>();

            var sqlState = (state) ? "ON" : "OFF";

            var query = $"SET IDENTITY_INSERT {tableName} {sqlState}";

            return ExecuteSql(query);
        }

        public int ExecuteSql(string query)
        {
            var noOfEffectedRows = 0;

            DbContext.Database.Connection.Open();
            noOfEffectedRows = DbContext.Database.ExecuteSqlCommand(query);
            DbContext.Database.Connection.Close();

            return noOfEffectedRows;
        }

        public IEnumerable<dynamic> ExecuteSqlQuery<T>(string query, Dictionary<string, object> paramerters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> ExecuteSqlStoreProcedure<T>(string procedureName, Dictionary<string, object> paramerters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> ExecutePaginatedDataStoreProcedure<T>(string procedureName, Dictionary<string, object> paramerters, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        //public IEnumerable<dynamic> ExecuteSqlQuery<T>(string query, Dictionary<string, object> paramerters)
        //{
        //    var data = DbContext.CollectionFromSql(query, paramerters);
        //    return data;
        //}

        //public IEnumerable<dynamic> ExecuteSqlStoreProcedure<T>(string procedureName, Dictionary<string, object> paramerters)
        //{
        //    var data = DbContext.CollectionFromStoreProcedure(procedureName, paramerters, false);
        //    return data;
        //}

        //public IEnumerable<dynamic> ExecutePaginatedDataStoreProcedure<T>(string procedureName, Dictionary<string, object> paramerters, out int totalRecords)
        //{
        //    var data = DbContext.CollectionFromPagedDataStoreProcedure(procedureName, paramerters, out totalRecords);
        //    return data;
        //}

        private static dynamic GetDataRow(DbDataReader dataReader)
        {
            var dataRow = new ExpandoObject() as IDictionary<string, object>;
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
                dataRow.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
            return dataRow;
        }


        string GetTableName<T>() where T : class
        {
            var context = ((IObjectContextAdapter)DbContext).ObjectContext;

            var sql = context.CreateObjectSet<T>().ToTraceString();
            var regex = new Regex("FROM\\s+(?<table>.+)\\s+AS");
            var match = regex.Match(sql);

            var table = match.Groups["table"].Value;
            return table;
        }

        public DbRawSqlQuery<T> StoredProcedure<T>(string procName, params SqlParameter[] parameters)
        {
            var procedureName = procName + " " + string.Join(",", parameters.Select(param => "@" + param.ParameterName));
            return DbContext.Database.SqlQuery<T>(procedureName, parameters);
        }

        protected IView<T> GetStandardView<T>() where T : class
        {
            return RepositoryProvider.GetRepositoryForView<T>();
        }

        protected IRepository<T> GetStandardRepository<T>() where T : class
        {
            return RepositoryProvider.GetRepositoryForEntityType<T>();
        }

        protected T GetRepository<T>() where T : class
        {
            return RepositoryProvider.GetRepository<T>();
        }

        protected abstract void CreateDbContext(string dbConnectionString);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                DbContext?.Dispose();
            }
        }
    }
}
