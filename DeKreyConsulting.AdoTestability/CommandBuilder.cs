using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using ParameterInitializer = System.Action<System.Data.Common.DbParameter>;

namespace DeKreyConsulting.AdoTestability
{
    public class CommandBuilder
    {
        private readonly IEnumerable<ParameterDefinition> parameterDefinitions;

        /// <summary>
        /// Constructs the command builder. Use CommandBuilderFactory instead.
        /// </summary>
        /// <param name="commandText">The command to execute, such as raw SQL or a stored procedure, depending on the command type</param>
        /// <param name="parameters">A list of parameter initializers, by name, to set types, directions, etc. This is copied to prevent modification in order to ensure
        /// deterministic behavior. (Slight performance hit, but these should be constructed statically.)</param>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandTimeout">The timeout duration, in seconds. Defaults to no timeout.</param>
        internal CommandBuilder(string commandText, IEnumerable<ParameterDefinition> parameterDefinitions, CommandType commandType, int commandTimeout)
        {
            this.CommandText = commandText;
            this.parameterDefinitions = parameterDefinitions;
            this.CommandType = commandType;
            this.CommandTimeout = commandTimeout;
            this.parameterDefinitions = parameterDefinitions;
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
        public IEnumerable<ParameterDefinition> Parameters => parameterDefinitions.ToArray();

        /// <summary>
        /// Builds a command given specific values
        /// </summary>
        /// <param name="connection">The connection that the command will be executed against</param>
        /// <param name="transaction">The transaction within which to execute the command</param>
        /// <returns>A new DbCommand that should be disposed of after use</returns>
        public DbCommand BuildFrom(DbConnection connection, DbTransaction? transaction = null)
        {
            var command = connection.CreateCommand();
            command.CommandText = CommandText;
            command.CommandType = CommandType;
            command.CommandTimeout = CommandTimeout;
            command.Connection = connection;
            command.Transaction = transaction;

            foreach (var param in parameterDefinitions)
            {
                command.Parameters.Add(param.BuildParameter(command));
            }
            return command;
        }

        /// <summary>
        /// Builds a command given specific values
        /// </summary>
        /// <param name="connection">The connection that the command will be executed against</param>
        /// <param name="parameterValues">The values of the parameters to be used, if any</param>
        /// <param name="transaction">The transaction within which to execute the command</param>
        /// <returns>A new DbCommand that should be disposed of after use</returns>
        public DbCommand BuildFrom(DbConnection connection, IList<object> parameterValues, DbTransaction? transaction = null)
        {
            var command = BuildFrom(connection, transaction);

            for (var i = 0; i < parameterValues.Count; i++)
            {
                command.Parameters[i].Value = parameterValues[i] ?? DBNull.Value;
            }

            return command;
        }

        /// <summary>
        /// Builds a command given specific values
        /// </summary>
        /// <param name="connection">The connection that the command will be executed against</param>
        /// <param name="parameterValues">The values of the parameters to be used, if any</param>
        /// <param name="transaction">The transaction within which to execute the command</param>
        /// <returns>A new DbCommand that should be disposed of after use</returns>
        public DbCommand BuildFrom(DbConnection connection, IReadOnlyDictionary<string, object> parameterValues, DbTransaction? transaction = null)
        {
            return BuildFrom(connection, transaction).ApplyParameters(parameterValues);
        }
    }
}