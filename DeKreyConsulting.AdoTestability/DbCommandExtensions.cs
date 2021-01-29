using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DeKreyConsulting.AdoTestability
{
    public static class DbCommandExtensions
    {
        public static DbCommand ApplyParameters(this DbCommand command, params KeyValuePair<string, object>[] parameterValues)
        {
            var parameters = command.Parameters.OfType<DbParameter>().Where(p => p.ParameterName != null).ToDictionary(p => p.ParameterName, p => p);
            foreach (var param in parameterValues)
            {
                parameters[param.Key].Value = param.Value;
            }
            return command;
        }
    }
}