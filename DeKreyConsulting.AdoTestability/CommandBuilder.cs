using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using ParameterInitializer = System.Action<System.Data.Common.DbParameter>;

namespace DeKreyConsulting.AdoTestability
{
    using IReadOnlyParameterInitializers = IReadOnlyDictionary<string, ParameterInitializer>;
    using ReadOnlyParameterInitializers = ReadOnlyDictionary<string, ParameterInitializer>;

    /// <summary>
    /// Encapsulates the basics of building an ADO.Net command in a functional way. The CommandBuilders should be constructed statically and re-used across
    /// multiple connections; it encapsulates connection-agnostic portions of the command as an object that can be re-used and builds a command given 
    /// instance-specific values.
    /// </summary>
    public class CommandBuilder
    {
        /// <summary>
        /// Constructs the command builder.
        /// </summary>
        /// <param name="commandText">The command to execute, such as raw SQL or a stored procedure, depending on the command type</param>
        /// <param name="parameters">A list of parameter initializers, by name, to set types, directions, etc. This is copied to prevent modification in order to ensure
        /// deterministic behavior. (Slight performance hit, but these should be constructed statically.)</param>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandTimeout">The timeout duration, in seconds. Defaults to no timeout.</param>
        public CommandBuilder(string commandText, IReadOnlyParameterInitializers parameters = null, CommandType commandType = CommandType.Text, int commandTimeout = 0)
        {
            this.CommandText = commandText;
            this.CommandType = commandType;
            this.CommandTimeout = commandTimeout;
            this.Parameters = parameters ?? new ReadOnlyParameterInitializers(new Dictionary<string, ParameterInitializer>());
        }

        /// <summary>
        /// Gets the command text specified by the constructor
        /// </summary>
        public string CommandText { get; }

        /// <summary>
        /// Gets the command type specified by the constructor
        /// </summary>
        public CommandType CommandType { get; }

        /// <summary>
        /// Gets the command timeout specified by the constructor
        /// </summary>
        public int CommandTimeout { get; }

        /// <summary>
        /// Gets a copy of the parameters specified by the constructor
        /// </summary>
        public IReadOnlyParameterInitializers Parameters { get; }

        /// <summary>
        /// Builds a command given specific values
        /// </summary>
        /// <param name="connection">The connection that the command will be executed against</param>
        /// <param name="parameterValues">The values of the parameters to be used, if any</param>
        /// <param name="transaction">The transaction within which to execute the command</param>
        /// <returns>A new DbCommand that should be disposed of after use</returns>
        public DbCommand BuildFrom(DbConnection connection, IReadOnlyDictionary<string, object> parameterValues = null, DbTransaction transaction = null)
        {
            try
            {
                var command = connection.CreateCommand();
                command.CommandText = CommandText;
                command.CommandType = CommandType;
                command.CommandTimeout = CommandTimeout;
                command.Connection = connection;
                command.Transaction = transaction;

                foreach (var param in Parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = param.Key;
                    param.Value(parameter);
                    command.Parameters.Add(parameter);
                }

                if (parameterValues != null)
                {
                    command.ApplyParameters(parameterValues);
                }

                return command;
            }
            catch (System.Exception ex)
            {
                throw new System.InvalidOperationException($"Unable to set up command: {CommandText}", ex);
            }
        }
    }
}
