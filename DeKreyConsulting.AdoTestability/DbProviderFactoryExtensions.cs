using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.Common
{
    public static class DbProviderFactoryExtensions
    {
        public static DbConnection CreateConnection(this DbProviderFactory providerFactory, string connectionString)
        {
            var connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
