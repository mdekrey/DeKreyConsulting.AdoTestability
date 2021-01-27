using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using ParameterInitializer = System.Action<System.Data.Common.DbParameter>;

namespace DeKreyConsulting.AdoTestability
{
    /// <summary>
    /// Encapsulates the basics of building an ADO.Net command in a functional way. The CommandBuilders should be constructed statically and re-used across
    /// multiple connections; it encapsulates connection-agnostic portions of the command as an object that can be re-used and builds a command given 
    /// instance-specific values.
    /// </summary>
    public class CommandBuilderFactory : System.Collections.IEnumerable
    {
        private bool? hasNamedParameter = null;
        private readonly List<ParameterDefinition> parameters = new List<ParameterDefinition>();
        private readonly string commandText;
        private readonly CommandType commandType;
        private readonly int commandTimeout;

        /// <summary>
        /// Constructs the command builder.
        /// </summary>
        /// <param name="commandText">The command to execute, such as raw SQL or a stored procedure, depending on the command type</param>
        /// <param name="parameters">A list of parameter initializers, by name, to set types, directions, etc. This is copied to prevent modification in order to ensure
        /// deterministic behavior. (Slight performance hit, but these should be constructed statically.)</param>
        /// <param name="commandType">The type of the command</param>
        /// <param name="commandTimeout">The timeout duration, in seconds. Defaults to no timeout.</param>
        public CommandBuilderFactory(string commandText, CommandType commandType = CommandType.Text, int commandTimeout = 0)
        {
            this.commandText = commandText;
            this.commandType = commandType;
            this.commandTimeout = commandTimeout;
        }

        public void Add(ParameterDefinition definition)
        {
            if (hasNamedParameter is bool v && v != (definition.Name is not null))
            {
                throw new ArgumentException("Must either have all unnamed parameters or all named parameters.");
            }
            hasNamedParameter = definition.Name is not null;
            parameters.Add(definition);
        }

        public void Add(string name, ParameterInitializer parameterInitializer) =>
            Add(new ParameterDefinition(name, parameterInitializer));
        public void Add(ParameterInitializer parameterInitializer) =>
            Add(new ParameterDefinition(null, parameterInitializer));

        public void Add(string name, DbType dbType) =>
            Add(name, p => p.DbType = dbType);
        public void Add(DbType dbType) =>
            Add(p => p.DbType = dbType);

        public void Add(string name, DbType dbType, ParameterInitializer parameterInitializer) =>
            Add(name, p => { p.DbType = dbType; parameterInitializer(p); });
        public void Add(DbType dbType, ParameterInitializer parameterInitializer) =>
            Add(p => { p.DbType = dbType; parameterInitializer(p); });

        public CommandBuilder Build()
        {
            return new CommandBuilder(commandText, parameters.ToArray(), commandType, commandTimeout);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return parameters.GetEnumerator();
        }
    }
}
