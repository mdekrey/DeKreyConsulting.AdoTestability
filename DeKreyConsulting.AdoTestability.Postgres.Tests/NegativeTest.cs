using DeKreyConsulting.AdoTestability.Postgres.Example;
using DeKreyConsulting.AdoTestability.Testing.Postgres;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeKreyConsulting.AdoTestability.Postgres.Tests
{
    public class NegativeTest
    {
        // See the readme for steps to run the tests
        const string connectionString = "Host=localhost;Port=65432;Username=postgres;Password=testdb!1";
        
        [Fact]
        public void FailedQueryTest()
        {
            try
            {
                ValidateCommand(new CommandBuilderFactory(commandText: @"SELECT 1 FROM NonExistantTable").Build());
                Assert.False(true, "SqlException was not thrown.");
            }
            catch (NpgsqlException)
            {
                // expected exception
            }
        }

        [Fact]
        public void TableScanTest()
        {
            try
            {
                new CommandBuilderFactory(commandText: @"SELECT 1 FROM People").Build().ValidateAndExplainCommand(BuildSqlConnection(), PostgresExtensions.EnsureNoSeqScan);
                Assert.False(true, "BadExecutionPlanException was not thrown.");
            }
            catch (BadExecutionPlanException)
            {
                // expected exception
            }
        }

        #region Helper methods

        private void ValidateCommand(CommandBuilder builder) =>
            builder.ValidateCommand(BuildSqlConnection());

        private static NpgsqlConnection BuildSqlConnection() =>
            (NpgsqlConnection)NpgsqlFactory.Instance.CreateConnection(connectionString);

        #endregion
    }
}
