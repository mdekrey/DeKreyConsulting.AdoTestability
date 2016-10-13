using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace DeKreyConsulting.AdoTestability.Example
{
    public class EmailManager
    {
        #region Commands

        // We place all SQL in publicly accessibly commands so they can be tested for validity
        // When we need specific parameter types, the type must be tested for; most derived types are sealed and cannot be mocked. The below example is gratuitous.

        public static readonly CommandBuilder FindPersonByIdCommand = new CommandBuilder(
            commandText: @"SELECT FullName, Email, OptOut
                           FROM People
                           WHERE Id=@Id",
            parameters: new Dictionary<string, Action<DbParameter>>
            {
                { "@Id", p => p.DbType = System.Data.DbType.Int32 },
            });
        public static readonly CommandBuilder FindPeopleByEmailCommand = new CommandBuilder(
            commandText: @"SELECT Id, FullName, OptOut
                           FROM People
                           WHERE Email=@Email",
            parameters: new Dictionary<string, Action<DbParameter>>
            {
                { "@Email", p => p.DbType = System.Data.DbType.String },
            });
        public static readonly CommandBuilder CreatePersonCommand = new CommandBuilder(
            commandText: @"INSERT INTO People (FullName, Email, OptOut)
                           VALUES (@FullName, @Email, 0);

                           SELECT LAST_INSERT_ROWID()",
            parameters: new Dictionary<string, Action<DbParameter>>
            {
                { "@FullName", p => p.DbType = System.Data.DbType.String },
                { "@Email", p => p.DbType = System.Data.DbType.String },
            });
        public static readonly CommandBuilder OptOutCommand = new CommandBuilder(
            commandText: @"UPDATE People 
                           SET OptOut = 1
                           WHERE Email = @Email;",
            parameters: new Dictionary<string, Action<DbParameter>>
            {
                { "@Email", p => p.DbType = System.Data.DbType.AnsiString },
            });
        public static readonly CommandBuilder GetOptedInCommand = new CommandBuilder(
            commandText: @"SELECT Id, FullName, Email, OptOut
                           FROM People
                           WHERE OptOut = 0",
            parameters: new Dictionary<string, Action<DbParameter>>
            {
            });

        #endregion
        
        private readonly DbProviderFactory dbProvider;
        private readonly string connectionString;

        public EmailManager(DbProviderFactory dbProvider, string connectionString)
        {
            this.dbProvider = dbProvider;
            this.connectionString = connectionString;
        }

        #region CreatePerson

        public async Task<Person> CreatePersonAsync(string fullName, string email)
        {
            using (var connection = dbProvider.CreateConnection(connectionString))
            using (var command = CreatePersonCommand.BuildFrom(connection, new Dictionary<string, object>
            {
                { "@FullName", fullName },
                { "@Email", email }
            }))
            {
                await connection.OpenAsync();
                return new Person(
                    id: Convert.ToInt32(await command.ExecuteScalarAsync()),
                    fullName: fullName,
                    email: email,
                    optOut: false);
            }
        }

        public Task<Person> CreatePersonFunctionalAsync(string fullName, string email) =>
            dbProvider.CreateOpenConnectionAndAsync(connectionString, conn =>
                CreatePersonCommand.BuildFrom(conn, new Dictionary<string, object>
                {
                    { "@FullName", fullName },
                    { "@Email", email }
                }).DisposeAfterAsync(async cmd =>
                    new Person(
                        id: Convert.ToInt32(await cmd.ExecuteScalarAsync()),
                        fullName: fullName,
                        email: email,
                        optOut: false)
                )
            );

        #endregion

        #region GetPersonById

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            using (var connection = dbProvider.CreateConnection(connectionString))
            using (var command = FindPersonByIdCommand.BuildFrom(connection, new Dictionary<string, object>
            {
                { "@Id", id },
            }))
            {
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new Person(
                            id: id,
                            fullName: Convert.ToString(reader["FullName"]),
                            email: Convert.ToString(reader["Email"]),
                            optOut: Convert.ToBoolean(reader["OptOut"]));
                    }
                }

                return null;
            }
        }

        public Task<Person> GetPersonByIdFunctionalAsync(int id) =>
            dbProvider.CreateOpenConnectionAndAsync(connectionString, conn =>
                FindPersonByIdCommand.BuildFrom(conn, new Dictionary<string, object>
                {
                    { "@Id", id },
                }).DisposeAfterAsync(async cmd =>
                    await (await cmd.ExecuteReaderAsync()).DisposeAfterAsync(async reader =>
                        (await reader.ReadRecordsAsync(record => Task.FromResult(new Person(
                            id: id,
                            fullName: Convert.ToString(reader["FullName"]),
                            email: Convert.ToString(reader["Email"]),
                            optOut: Convert.ToBoolean(reader["OptOut"]))))).FirstOrDefault())
                )
            );

        #endregion

        #region GetPersonByEmail

        public async Task<IEnumerable<Person>> GetPeopleByEmailAsync(string email)
        {
            using (var connection = dbProvider.CreateConnection(connectionString))
            using (var command = FindPeopleByEmailCommand.BuildFrom(connection, new Dictionary<string, object>
            {
                { "@Email", email },
            }))
            {
                connection.Open();
                var list = new List<Person>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new Person(
                            id: Convert.ToInt32(reader["Id"]),
                            fullName: Convert.ToString(reader["FullName"]),
                            email: email,
                            optOut: Convert.ToBoolean(reader["OptOut"])));
                    }
                }

                return list;
            }
        }

        public Task<IEnumerable<Person>> GetPeopleByEmailFunctionalAsync(string email) =>
            dbProvider.CreateOpenConnectionAndAsync(connectionString, conn =>
                FindPeopleByEmailCommand.BuildFrom(conn, new Dictionary<string, object>
                {
                    { "@Email", email },
                }).DisposeAfterAsync(async cmd =>
                    await (await cmd.ExecuteReaderAsync()).DisposeAfterAsync(async reader =>
                        (await reader.ReadRecordsAsync(record => Task.FromResult(new Person(
                            id: Convert.ToInt32(reader["Id"]),
                            fullName: Convert.ToString(reader["FullName"]),
                            email: email,
                            optOut: Convert.ToBoolean(reader["OptOut"]))))))
                )
            );

        #endregion

        #region OptOutByEmail

        public async Task<int> OptOutByEmailAsync(string email)
        {
            using (var connection = dbProvider.CreateConnection(connectionString))
            using (var command = OptOutCommand.BuildFrom(connection, new Dictionary<string, object>
            {
                { "@Email", email },
            }))
            {
                connection.Open();
                return await command.ExecuteNonQueryAsync();
            }
        }

        public Task<int> OptOutByEmailFunctionalAsync(string email) =>
            dbProvider.CreateOpenConnectionAndAsync(connectionString, conn =>
                OptOutCommand.BuildFrom(conn, new Dictionary<string, object>
                {
                    { "@Email", email },
                }).DisposeAfterAsync(async cmd =>
                    await cmd.ExecuteNonQueryAsync()
                )
            );

        #endregion

        #region GetOptedIn

        public async Task<IEnumerable<Person>> GetOptedInAsync()
        {
            using (var connection = dbProvider.CreateConnection(connectionString))
            using (var command = GetOptedInCommand.BuildFrom(connection))
            {
                connection.Open();
                var list = new List<Person>();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new Person(
                            id: Convert.ToInt32(reader["Id"]),
                            fullName: Convert.ToString(reader["FullName"]),
                            email: Convert.ToString(reader["Email"]),
                            optOut: Convert.ToBoolean(reader["OptOut"])));
                    }
                }

                return list;
            }
        }

        public Task<IEnumerable<Person>> GetOptedInFunctionalAsync() =>
            dbProvider.CreateOpenConnectionAndAsync(connectionString, conn =>
                GetOptedInCommand.BuildFrom(conn).DisposeAfterAsync(async cmd =>
                    await (await cmd.ExecuteReaderAsync()).DisposeAfterAsync(async reader =>
                        (await reader.ReadRecordsAsync(record => Task.FromResult(new Person(
                            id: Convert.ToInt32(reader["Id"]),
                            fullName: Convert.ToString(reader["FullName"]),
                            email: Convert.ToString(reader["Email"]),
                            optOut: Convert.ToBoolean(reader["OptOut"]))))))
                )
            );

        #endregion
    }
}
