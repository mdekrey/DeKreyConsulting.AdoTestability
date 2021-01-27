using System;
using System.Collections.Generic;
using System.Data.Common;

namespace DeKreyConsulting.AdoTestability
{
    public static class DbCommandExtensions
    {
        public static DbCommand ApplyParameters(this DbCommand command, IReadOnlyDictionary<string, object> parameterValues)
        {
            foreach (DbParameter param in command.Parameters)
            {
                param.Value = (parameterValues.TryGetValue(param.ParameterName, out var v)
                    ? v
                    : null) ?? DBNull.Value;
            }
            return command;
        }
    }
}