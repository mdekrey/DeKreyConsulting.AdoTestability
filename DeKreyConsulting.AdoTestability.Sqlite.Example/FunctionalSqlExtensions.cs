using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DeKreyConsulting.AdoTestability.Example
{
    public static class FunctionalSqlExtensions
    {
        public static Task<TReturn> CreateOpenConnectionAndAsync<TReturn>(this DbProviderFactory dbProvider, string connectionString, Func<DbConnection, Task<TReturn>> action) =>
            dbProvider.CreateConnection(connectionString)
                .OpenAndDisposeAfterAsync(action);

        public static Task<TReturn> OpenAndDisposeAfterAsync<TConnection, TReturn>(this TConnection connect, Func<TConnection, Task<TReturn>> action)
            where TConnection : DbConnection =>
            connect.DisposeAfterAsync(async c =>
            {
                await c.OpenAsync();
                return await action(c);
            });

        public static async Task<TReturn> DisposeAfterAsync<TDisposable, TReturn>(this TDisposable disposable, Func<TDisposable, Task<TReturn>> action)
            where TDisposable : IDisposable
        {
            using (disposable)
            {
                return await action(disposable);
            }
        }

        public static async Task<IEnumerable<TReturn>> ReadRecordsAsync<TReturn>(this DbDataReader reader, Func<DbDataReader, Task<TReturn>> action)
        {
            var result = new List<TReturn>();
            while (await reader.ReadAsync())
            {
                result.Add(await action(reader));
            }
            return result;
        }
    }
}
