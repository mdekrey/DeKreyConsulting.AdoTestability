using DeKreyConsulting.AdoTestability.Example;
using DeKreyConsulting.AdoTestability.Testing.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeKreyConsulting.AdoTestability.Tests
{
    public class NegativeTest
    {
        // Install the dacpac ("Deploy Data-tier Application") produced by the SampleDatabase project
        const string connectionString = "Server=.\\SQLExpress;Database=adotestability;Trusted_Connection=True;";

        [Fact]
        public void OptOutIsNotSingleExplainTest()
        {
            try
            {
                ExplainSingleResult(EmailManager.OptOutCommand);
                Assert.False(true, "BadExecutionPlanException was not thrown.");
            }
            catch (BadExecutionPlanException)
            {
                // expected exception
            }
        }

        [Fact]
        public void FailedQueryTest()
        {
            try
            {
                ExplainSingleResult(new CommandBuilder(commandText: @"SELECT 1 FROM [dbo].[NonExistantTable]"));
                Assert.False(true, "SqlException was not thrown.");
            }
            catch (SqlException)
            {
                // expected exception
            }
        }

        #region Helper methods

        private void ExplainSingleResult(CommandBuilder builder) =>
            builder.ExplainSingleResult(BuildSqlConnection());
        private void ExplainMultipleResult(CommandBuilder builder) =>
            builder.ExplainMultipleResult(BuildSqlConnection());

        private static SqlConnection BuildSqlConnection() =>
            (SqlConnection)SqlClientFactory.Instance.CreateConnection(connectionString);

        #endregion
    }
}
