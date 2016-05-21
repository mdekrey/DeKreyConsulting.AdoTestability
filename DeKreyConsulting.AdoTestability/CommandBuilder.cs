using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ParameterInitializer = System.Action<System.Data.Common.DbParameter>;

namespace DeKreyConsulting.AdoTestability
{
    using IReadOnlyParameterInitializers = IReadOnlyDictionary<string, ParameterInitializer>;
    using ReadOnlyParameterInitializers = ReadOnlyDictionary<string, ParameterInitializer>;

    public class CommandBuilder
    {
        public CommandBuilder(string commandText, IReadOnlyParameterInitializers parameters)
        {
            this.CommandText = commandText;
            this.Parameters = new ReadOnlyParameterInitializers(parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        public string CommandText { get; }
        public IReadOnlyParameterInitializers Parameters { get; }

        public DbCommand BuildFrom(DbConnection connection, IReadOnlyDictionary<string, object> parameterValues)
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

        public static CommandBuilder Construct<TParameter>(string commandText, IReadOnlyDictionary<string, Action<TParameter>> parameters)
            where TParameter : DbParameter =>
            new CommandBuilder(
                commandText, 
                parameters
                    .ToDictionary(
                        keySelector: kvp => kvp.Key, 
                        elementSelector: kvp => (ParameterInitializer)(parameter => kvp.Value(parameter as TParameter))));
    }
}
