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
        public static void Explain(this CommandBuilder command, Microsoft.Data.Sqlite.SqliteConnection connection)
        {
            using (connection)
            using (var cmd = command.BuildFrom(connection, command.Parameters.ToDictionary(kvp => kvp.Key, kvp => (object)DBNull.Value)))
            {
                cmd.CommandText = "EXPLAIN QUERY PLAN " + cmd.CommandText;

                connection.Open();

                cmd.ExecuteNonQuery();
            }

            // Sqlite has no documented query plan result; we cannot really verify anything other than standard syntax
        }
    }
}
