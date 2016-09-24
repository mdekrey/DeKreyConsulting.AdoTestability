using DeKreyConsulting.AdoTestability.Testing.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DeKreyConsulting.AdoTestability
{
    public static class SqlClientExtensions
    {
        public static void Explain(this CommandBuilder command, System.Data.SqlClient.SqlConnection connection, Action<XmlDocument> analyzeSqlExecutionPlan)
        {
            string content;

            using (connection)
            using (var beginExplain = new System.Data.SqlClient.SqlCommand(@"SET SHOWPLAN_XML ON", connection))
            using (var endExplain = new System.Data.SqlClient.SqlCommand(@"SET SHOWPLAN_XML OFF", connection))
            using (var cmd = command.BuildFrom(connection, command.Parameters.ToDictionary(kvp => kvp.Key, kvp => (object)DBNull.Value)))
            {
                if (cmd.Parameters.Count > 0)
                {
                    cmd.CommandText = "DECLARE " + string.Join(", ", cmd.Parameters.OfType<System.Data.SqlClient.SqlParameter>().Select(p => p.ParameterName + " " + p.SqlDbType.ToString("g"))) + ";\r\n" + cmd.CommandText;
                    cmd.Parameters.Clear();
                }

#if NET451
                connection.InfoMessage += (sender, e) => System.Diagnostics.Trace.WriteLine(e.Message);
#endif
                connection.Open();

                beginExplain.ExecuteNonQuery();

                content = cmd.ExecuteScalar().ToString();

                endExplain.ExecuteNonQuery();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            analyzeSqlExecutionPlan(doc);
        }

        public static void ExplainSingleResult(this CommandBuilder command, System.Data.SqlClient.SqlConnection connection) =>
            command.Explain(connection, AnalyzeSingleResultExecutionPlan);
        public static void ExplainMultipleResult(this CommandBuilder command, System.Data.SqlClient.SqlConnection connection) =>
            command.Explain(connection, AnalyzeMultipleResultExecutionPlan);

        public static void AnalyzeSingleResultExecutionPlan(XmlDocument doc) =>
            ScanForBadXPaths(doc, new List<string>()
            {
                "//*[@PhysicalOp='Table Scan']",
                "//*[@PhysicalOp='Clustered Index Scan' or @PhysicalOp='Clustered Index Scan']",
            });

        public static void AnalyzeMultipleResultExecutionPlan(XmlDocument doc) =>
            ScanForBadXPaths(doc, new List<string>()
            {
                "//*[@PhysicalOp='Table Scan']",
            });

        public static void ScanForBadXPaths(XmlDocument doc, IEnumerable<string> badXpaths)
        {
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("", "http://schemas.microsoft.com/sqlserver/2004/07/showplan");
            foreach (var badXpath in badXpaths)
            {
                var elems = doc.SelectNodes(badXpath, namespaceManager).OfType<XmlNode>();
                if (elems.Any())
                {
                    throw new BadExecutionPlanException("Execution plan had disallowed entry - " + badXpath + ":\r\n" + elems.First().OuterXml);
                }
            }
        }
    }
}
