using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using ParameterInitializer = System.Action<System.Data.Common.DbParameter>;

namespace DeKreyConsulting.AdoTestability
{
    public class ParameterDefinition
    {
        public ParameterDefinition(string? name, ParameterInitializer parameterInitializer)
        {
            Name = name;
            ParameterInitializer = parameterInitializer;
        }

        public string? Name { get; }
        public ParameterInitializer ParameterInitializer { get; }

        internal DbParameter BuildParameter(DbCommand command)
        {
            var parameter = command.CreateParameter();
            if (Name != null)
            {
                parameter.ParameterName = Name;
            }
            ParameterInitializer(parameter);
            return parameter;
        }
    }
}
