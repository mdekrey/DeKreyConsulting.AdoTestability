using DeKreyConsulting.AdoTestability.Testing.Postgres;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DeKreyConsulting.AdoTestability
{
    public delegate IEnumerable<JToken> GetBadTokensDelegate(JToken explainPlan);
    public delegate void HandleExplainPlan(JToken explainPlan);

    public static class PostgresExtensions
    {
        public static void Explain(this CommandBuilder command, NpgsqlConnection connection, HandleExplainPlan analyzeSqlExecutionPlan)
        {
            string content = "";

            using (connection)
            using (var cmd = command.BuildFrom(connection, command.Parameters.ToDictionary(kvp => kvp.Key, kvp => (object)DBNull.Value)))
            {
                cmd.CommandText = "EXPLAIN (FORMAT JSON, VERBOSE) " + string.Join("; EXPLAIN", cmd.CommandText.Split(';'));
                foreach (NpgsqlParameter param in cmd.Parameters)
                {
                    param.Direction = System.Data.ParameterDirection.Input;
                }
                
                connection.Open();

                content = cmd.ExecuteScalar().ToString();
            }
            
            analyzeSqlExecutionPlan(JToken.Parse(content));
        }

        public static void ValidateCommand(this CommandBuilder command, NpgsqlConnection connection) =>
            command.Explain(connection, doc => { });

        /// <summary>
        /// Note that you need to have a populated database to verify explain plans. You can make sure it's doing what you expect with the HandleExplainPlan
        /// callback. There's a few helper methods for that, such as EnsureNoSeqScan; see the source on github for examples.
        /// </summary>
        public static void ValidateAndExplainCommand(this CommandBuilder command, NpgsqlConnection connection, HandleExplainPlan handleExplain) =>
            command.Explain(connection, handleExplain);

        public static void EnsureNoSeqScan(JToken doc) =>
            ScanForBadXPaths(doc, new List<GetBadTokensDelegate>()
            {
                EnsureNoNodeTypesOf("Seq Scan"),
            });

        public static GetBadTokensDelegate EnsureNoNodeTypesOf(string v)
        {
            return (node) =>
            {
                return from child in node.SelectTokens("$..Plans[*]").Concat(node.SelectTokens("$..Plan"))
                       where child.SelectToken("$['Node Type']").ToString() == v
                       select child;
            };
        }
        
        public static void ScanForBadXPaths(JToken doc, IEnumerable<GetBadTokensDelegate> badTokenSelectors)
        {
            foreach (var badTokenSelector in badTokenSelectors)
            {
                var elems = badTokenSelector(doc).Take(1).ToArray();
                if (elems.Length > 0)
                {
                    throw new BadExecutionPlanException("Execution plan had disallowed entry - " + badTokenSelector + ":\r\n" + elems[0].ToString());
                }
            }
        }
    }
}
