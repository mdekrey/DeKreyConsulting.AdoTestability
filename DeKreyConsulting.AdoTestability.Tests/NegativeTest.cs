using DeKreyConsulting.AdoTestability.Example;
using DeKreyConsulting.AdoTestability.Testing.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeKreyConsulting.AdoTestability.Tests
{
    public class NegativeTest
    {
        // See the readme in the SampleDatabase project to set up a docker container
        const string connectionString = "Server=localhost,11433;Database=adotestability;User Id=sa;Password=weakPASSw0rd;";

        [Fact]
        public void OptOutIsNotSingleExplainTest()
        {
            Assert.Throws<BadExecutionPlanException>(() => ExplainSingleResult(EmailManager.OptOutCommand));
        }

        [Fact]
        public void FailedQueryTest()
        {
            Assert.Throws<SqlException>(() => ExplainSingleResult(new CommandBuilderFactory(commandText: @"SELECT 1 FROM [dbo].[NonExistantTable]").Build()));
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
