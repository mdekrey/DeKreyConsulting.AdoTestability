using DeKreyConsulting.AdoTestability.Testing.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DeKreyConsulting.AdoTestability
{
    public static class SqliteClientExtensions
    {
        public static void Explain(this CommandBuilder command, Microsoft.Data.Sqlite.SqliteConnection connection, Action<string> analyzeSqliteExecutionPlan)
        {
            string content;

            using (connection)
            using (var cmd = command.BuildFrom(connection, command.Parameters.ToDictionary(kvp => kvp.Key, kvp => (object)DBNull.Value)))
            {
                cmd.CommandText = "EXPLAIN QUERY PLAN " + cmd.CommandText + "; SELECT 1";

                connection.Open();

                content = cmd.ExecuteScalar().ToString();
            }

            analyzeSqliteExecutionPlan(content);
        }

        public static void ExplainSingleResult(this CommandBuilder command, Microsoft.Data.Sqlite.SqliteConnection connection) =>
            command.Explain(connection, AnalyzeSingleResultExecutionPlan);
        public static void ExplainMultipleResult(this CommandBuilder command, Microsoft.Data.Sqlite.SqliteConnection connection) =>
            command.Explain(connection, AnalyzeMultipleResultExecutionPlan);

        public static void AnalyzeSingleResultExecutionPlan(string doc)
        {

        }

        public static void AnalyzeMultipleResultExecutionPlan(string doc)
        {

        }

    }
}
