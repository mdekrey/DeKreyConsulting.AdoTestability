using DeKreyConsulting.AdoTestability.Postgres.Example;
using DeKreyConsulting.AdoTestability.Testing.Stubs;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeKreyConsulting.AdoTestability.Postgres.Tests
{
    using Npgsql;
    using Testing.Moq;
    using CommandSetup = Dictionary<CommandBuilder, SetupCommandBuilderMock>;

    public class EmailManagerTest
    {
        // See the readme for steps to run the tests
        const string connectionString = "Host=localhost;Port=65432;Username=postgres;Password=testdb!1";

        #region Explain Tests

        // The following tests run explain plans on the sql via the command builders.
        // `ExplainSingleResult` ensures that a query can only affect/return one row.
        // `ExplainMultipleResult` allows a query to affect/return multiple rows.
        // Running explain plans not only verifies that the queries are efficient, but that they actually are valid queries, too.

        [Fact]
        public void FindUserByIdExplainTest() =>
            ValidateCommand(EmailManager.FindPersonByIdCommand);


        [Fact]
        public void CreateUserExplainTest() =>
            ValidateCommand(EmailManager.CreatePersonCommand);


        [Fact]
        public void FindUserByEmailCommandExplainTest() =>
            ValidateCommand(EmailManager.FindPeopleByEmailCommand);


        [Fact]
        public void OptOutExplainTest() =>
            ValidateCommand(EmailManager.OptOutCommand);


        [Fact]
        public void GetOptedInExplainTest() =>
            ValidateCommand(EmailManager.GetOptedInCommand);

        #endregion

        #region CreatePerson

        // We can test queries by providing mocks; mock providers or mock connections, as the class has been created, so long as the generic interface has been used.
        // The test utilities provided capture parameters in and can return specific results.

        [Fact]
        public Task CreatePersonTest() =>
            CreatePerson((mgr, name, email) => mgr.CreatePersonAsync(name, email));

        [Fact]
        public Task CreatePersonFunctionalTest() =>
            CreatePerson((mgr, name, email) => mgr.CreatePersonFunctionalAsync(name, email));

        private async Task CreatePerson(Func<EmailManager, string, string, Task<Person>> createPerson)
        {
            const int id = 17;
            const string name = "First Last";
            const string email = "test@example.com";
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { EmailManager.CreatePersonCommand, (mockCmd, record) => mockCmd.Setup(cmd => cmd.ExecuteScalarAsync(AnyCancellationToken)).ReturnsWithDelay(id).Callback(record) }, });

            var person = await createPerson(CreateEmailManager(mocks.ProviderFactory.Object), name, email);

            mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
            mocks.Commands[EmailManager.CreatePersonCommand].Verify(command => command.ExecuteScalarAsync(AnyCancellationToken), Times.Once());

            Assert.Equal(1, mocks.Executions[EmailManager.CreatePersonCommand].Count);
            Assert.Equal(mocks.Executions[EmailManager.CreatePersonCommand][0]["@FullName"], name);
            Assert.Equal(mocks.Executions[EmailManager.CreatePersonCommand][0]["@Email"], email);
            Assert.Equal(id, person.Id);
            Assert.Equal(name, person.FullName);
            Assert.Equal(email, person.Email);
            Assert.Equal(false, person.OptOut);
        }

        [Fact]
        public async Task ActualCreatePersonTest()
        {
            var mgr = new EmailManager(NpgsqlFactory.Instance, connectionString);
            var person = await mgr.CreatePersonAsync("First Last", "first@last.com");

            Assert.NotEqual(0, person.Id);
            Assert.Equal("First Last", person.FullName);
            Assert.Equal("first@last.com", person.Email);
        }

        #endregion

        #region GetPersonById

        [Fact]
        public Task GetPersonByIdTest() =>
            GetPersonById((mgr, id) => mgr.GetPersonByIdAsync(id));

        [Fact]
        public Task GetPersonByIdFunctionalTest() =>
            GetPersonById((mgr, id) => mgr.GetPersonByIdFunctionalAsync(id));

        private async Task GetPersonById(Func<EmailManager, int, Task<Person>> getPerson)
        {
            const int id = 17;
            const string name = "First Last";
            const string email = "test@example.com";
            const bool optOut = false;
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup
            {
                {
                    EmailManager.FindPersonByIdCommand,
                    (mockCmd, record) => mockCmd.Setup(cmd => cmd.PublicExecuteDbDataReaderAsync(System.Data.CommandBehavior.Default, AnyCancellationToken)).ReturnsWithDelay(new FakeDataReader(new[] {
                        new Dictionary<string, object>
                        {
                            { "FullName", name },
                            { "Email", email },
                            { "OptOut", optOut },
                        }
                    })).Callback(record)
                },
            });

            var person = await getPerson(CreateEmailManager(mocks.ProviderFactory.Object), id);

            mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
            mocks.Commands[EmailManager.FindPersonByIdCommand].Verify(command => command.PublicExecuteDbDataReaderAsync(System.Data.CommandBehavior.Default, AnyCancellationToken), Times.Once());

            Assert.Equal(1, mocks.Executions[EmailManager.FindPersonByIdCommand].Count);
            Assert.Equal(mocks.Executions[EmailManager.FindPersonByIdCommand][0]["@Id"], id);
            Assert.Equal(id, person.Id);
            Assert.Equal(name, person.FullName);
            Assert.Equal(email, person.Email);
            Assert.Equal(false, person.OptOut);
        }

        #endregion

        #region GetPeopleByEmail

        [Fact]
        public Task GetPeopleByEmailTest() =>
            GetPeopleByEmail((mgr, email) => mgr.GetPeopleByEmailAsync(email));

        [Fact]
        public Task GetPeopleByEmailFunctionalTest() =>
            GetPeopleByEmail((mgr, email) => mgr.GetPeopleByEmailFunctionalAsync(email));

        private async Task GetPeopleByEmail(Func<EmailManager, string, Task<IEnumerable<Person>>> getPeople)
        {
            const string email = "test@example.com";
            const int id1 = 17;
            const string name1 = "First Last";
            const bool optOut1 = false;
            const int id2 = 212;
            const string name2 = "Other Person";
            const bool optOut2 = true;
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup
            {
                {
                    EmailManager.FindPeopleByEmailCommand,
                    (mockCmd, record) => mockCmd.Setup(cmd => cmd.PublicExecuteDbDataReaderAsync(System.Data.CommandBehavior.Default, AnyCancellationToken)).ReturnsWithDelay(new FakeDataReader(new[] {
                        new Dictionary<string, object>
                        {
                            { "Id", id1 },
                            { "FullName", name1 },
                            { "OptOut", optOut1 },
                        },
                        new Dictionary<string, object>
                        {
                            { "Id", id2 },
                            { "FullName", name2 },
                            { "OptOut", optOut2 },
                        },
                    })).Callback(record)
                },
            });

            var people = await getPeople(CreateEmailManager(mocks.ProviderFactory.Object), email);

            mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
            mocks.Commands[EmailManager.FindPeopleByEmailCommand].Verify(command => command.PublicExecuteDbDataReaderAsync(System.Data.CommandBehavior.Default, AnyCancellationToken), Times.Once());

            Assert.Equal(1, mocks.Executions[EmailManager.FindPeopleByEmailCommand].Count);
            Assert.Equal(mocks.Executions[EmailManager.FindPeopleByEmailCommand][0]["@Email"], email);

            var results = people.ToArray();
            Assert.Equal(id1, results[0].Id);
            Assert.Equal(name1, results[0].FullName);
            Assert.Equal(email, results[0].Email);
            Assert.Equal(optOut1, results[0].OptOut);
            Assert.Equal(id2, results[1].Id);
            Assert.Equal(name2, results[1].FullName);
            Assert.Equal(email, results[1].Email);
            Assert.Equal(optOut2, results[1].OptOut);
        }

        #endregion

        #region OptOutByEmail

        [Fact]
        public Task OptOutByEmailTest() =>
            OptOutByEmail((mgr, email) => mgr.OptOutByEmailAsync(email));

        [Fact]
        public Task OptOutByEmailFunctionalTest() =>
            OptOutByEmail((mgr, email) => mgr.OptOutByEmailFunctionalAsync(email));

        private async Task OptOutByEmail(Func<EmailManager, string, Task<int>> optOut)
        {
            const string email = "test@example.com";
            const int expectedResults = 2;
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { EmailManager.OptOutCommand, (mockCmd, record) => mockCmd.Setup(cmd => cmd.ExecuteNonQueryAsync(AnyCancellationToken)).ReturnsWithDelay(expectedResults).Callback(record) }, });

            var actualResults = await optOut(CreateEmailManager(mocks.ProviderFactory.Object), email);

            mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
            mocks.Commands[EmailManager.OptOutCommand].Verify(command => command.ExecuteNonQueryAsync(AnyCancellationToken), Times.Once());

            Assert.Equal(1, mocks.Executions[EmailManager.OptOutCommand].Count);
            Assert.Equal(mocks.Executions[EmailManager.OptOutCommand][0]["@Email"], email);

            Assert.Equal(expectedResults, actualResults);
        }

        #endregion

        #region GetOptedIn

        [Fact]
        public Task GetOptedInTest() =>
            GetOptedIn((mgr) => mgr.GetOptedInAsync());

        [Fact]
        public Task GetOptedInFunctionalTest() =>
            GetOptedIn((mgr) => mgr.GetOptedInFunctionalAsync());

        private async Task GetOptedIn(Func<EmailManager, Task<IEnumerable<Person>>> optOut)
        {
            const string email1 = "test@example.com";
            const int id1 = 17;
            const string name1 = "First Last";
            const bool optOut1 = false;
            const string email2 = "test@example.com";
            const int id2 = 212;
            const string name2 = "Other Person";
            const bool optOut2 = true;
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup
            {
                {
                    EmailManager.GetOptedInCommand,
                    (mockCmd, record) => mockCmd.Setup(cmd => cmd.PublicExecuteDbDataReaderAsync(System.Data.CommandBehavior.Default, AnyCancellationToken)).ReturnsWithDelay(new FakeDataReader(new[] {
                        new Dictionary<string, object>
                        {
                            { "Id", id1 },
                            { "FullName", name1 },
                            { "Email", email1 },
                            { "OptOut", optOut1 },
                        },
                        new Dictionary<string, object>
                        {
                            { "Id", id2 },
                            { "FullName", name2 },
                            { "Email", email2 },
                            { "OptOut", optOut2 },
                        },
                    })).Callback(record)
                },
            });

            var people = await optOut(CreateEmailManager(mocks.ProviderFactory.Object));

            mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
            mocks.Commands[EmailManager.GetOptedInCommand].Verify(command => command.PublicExecuteDbDataReaderAsync(System.Data.CommandBehavior.Default, AnyCancellationToken), Times.Once());

            Assert.Equal(1, mocks.Executions[EmailManager.GetOptedInCommand].Count);

            var results = people.ToArray();
            Assert.Equal(id1, results[0].Id);
            Assert.Equal(name1, results[0].FullName);
            Assert.Equal(email1, results[0].Email);
            Assert.Equal(optOut1, results[0].OptOut);
            Assert.Equal(id2, results[1].Id);
            Assert.Equal(name2, results[1].FullName);
            Assert.Equal(email2, results[1].Email);
            Assert.Equal(optOut2, results[1].OptOut);
        }

        #endregion

        #region Helper methods

        private static CancellationToken AnyCancellationToken =>
            It.IsAny<CancellationToken>();

        private EmailManager CreateEmailManager(DbProviderFactory providerFactory) =>
            new EmailManager(providerFactory, connectionString);

        private void ValidateCommand(CommandBuilder builder) =>
            builder.ValidateCommand(BuildSqlConnection());

        private static NpgsqlConnection BuildSqlConnection() =>
            (NpgsqlConnection)NpgsqlFactory.Instance.CreateConnection(connectionString);

        #endregion

    }
}
