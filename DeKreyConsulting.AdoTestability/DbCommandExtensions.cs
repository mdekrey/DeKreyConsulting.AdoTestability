using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.Common
{
    public static class DbCommandExtensions
    {
        public static void ApplyParameters(this DbCommand command, IReadOnlyDictionary<string, object> parameterValues)
        {
            foreach (var param in parameterValues)
            {
                command.Parameters[param.Key].Value = param.Value;
            }
        }
    }
}
