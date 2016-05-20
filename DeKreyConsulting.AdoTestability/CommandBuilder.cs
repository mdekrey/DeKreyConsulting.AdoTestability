using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DeKreyConsulting.AdoTestability
{

    public class CommandBuilder
    {
        public CommandBuilder(string commandText, Dictionary<string, Action<DbParameter>> parameters)
        {
            this.CommandText = commandText;
            this.Parameters = new ReadOnlyDictionary<string, Action<DbParameter>>(new Dictionary<string, Action<DbParameter>>(parameters));
        }

        public string CommandText { get; private set; }
        public IReadOnlyDictionary<string, Action<DbParameter>> Parameters { get; private set; }

        public DbCommand BuildFrom(DbConnection connection, Dictionary<string, object> parameterValues)
        {
            var command = connection.CreateCommand();
            command.CommandText = CommandText;
            command.Connection = connection;

            foreach (var param in Parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Key;
                param.Value(parameter);
                command.Parameters.Add(parameter);
            }

            if (parameterValues != null)
            {
                foreach (var param in parameterValues)
                {
                    command.Parameters[param.Key].Value = param.Value;
                }
            }

            return command;
        }
    }
}
