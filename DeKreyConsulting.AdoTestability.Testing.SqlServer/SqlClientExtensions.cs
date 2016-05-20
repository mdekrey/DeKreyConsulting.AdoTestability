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
        public static void Explain(this CommandBuilder command, System.Data.SqlClient.SqlConnection connection, bool allowMultipleResults = false)
        {
            string content;

            using (connection)
            using (var beginExplain = new System.Data.SqlClient.SqlCommand(@"SET SHOWPLAN_XML ON", connection))
            using (var endExplain = new System.Data.SqlClient.SqlCommand(@"SET SHOWPLAN_XML OFF", connection))
            using (var cmd = command.BuildFrom(connection, null))
            {
                if (cmd.Parameters.Count > 0)
                {
                    cmd.CommandText = "DECLARE " + string.Join(", ", cmd.Parameters.OfType<System.Data.SqlClient.SqlParameter>().Select(p => p.ParameterName + " " + p.SqlDbType.ToString("g"))) + ";\r\n" + cmd.CommandText;
                    cmd.Parameters.Clear();
                }

#if !DOTNET5_4
                connection.InfoMessage += (sender, e) => System.Diagnostics.Trace.WriteLine(e.Message);
#endif
                connection.Open();

                beginExplain.ExecuteNonQuery();

                content = cmd.ExecuteScalar().ToString();

                endExplain.ExecuteNonQuery();
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            AnalyzeExecutionPlan(allowMultipleResults, doc);
        }

        private static void AnalyzeExecutionPlan(bool allowMultipleResults, XmlDocument doc)
        {
            var badXpaths = new List<string>()
            {
                "//*[@PhysicalOp='Table Scan']",
            };
            if (!allowMultipleResults)
            {
                badXpaths.AddRange(new[]
                {
                    "//*[@PhysicalOp='Clustered Index Scan' or @PhysicalOp='Clustered Index Scan']"
                });
            }

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("", "http://schemas.microsoft.com/sqlserver/2004/07/showplan");
            foreach (var badXpath in badXpaths)
            {
                var elems = doc.SelectNodes(badXpath, namespaceManager) as IEnumerable<XmlNode>;
                if (elems.Any())
                {
                    throw new BadExecutionPlanException("Execution plan had disallowed entry - " + badXpath + ":\r\n" + elems.First().ToString());
                }
            }
        }
    }
}
