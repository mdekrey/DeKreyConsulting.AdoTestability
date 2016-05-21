using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.Common
{
    /// <summary>
    /// Extensions for the DbProviderFactory
    /// </summary>
    public static class AdoTestingDbProviderFactoryExtensions
    {
        /// <summary>
        /// Creates a connection with a specific connection string. Does not open the connection.
        /// </summary>
        /// <param name="providerFactory">A DbProviderFactory. Learn about DbProviderFactories from ADO.Net!</param>
        /// <param name="connectionString">The connection string to use for the connection</param>
        /// <returns>A new DbConnection</returns>
        public static DbConnection CreateConnection(this DbProviderFactory providerFactory, string connectionString)
        {
            var connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }
    }
}
