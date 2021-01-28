# AdoTestability
A testability framework for ADO.Net

Sometimes, despite all the ORM frameworks that exist, you have to connect to a database. In .NET,
it's easy to use ADO.NET. Unfortunately, ADO.NET is not a very testable framework the way most
developers use it. This framework is intended to make it easy to write secure, testable queries
in a simple and straightforward way.

This framework has come in a few parts, to reduce dependencies. Currently it supports .Net
Framework 4.5.2 and .NET Core (via .NET Standard 1.2).

## DeKreyConsulting.AdoTestability
The core project in the framework includes a simple `CommandBuilder` that can assemble a
`System.Data.Common.DbCommand` from common configuration and connection-specifc configuration.
For example, you create a `CommandBuilder` statically and persist it:

```cs
public static readonly CommandBuilder FindPersonByIdCommand = new CommandBuilderFactory(
    commandText: @"SELECT FullName, Email, OptOut
                   FROM [dbo].[People]
                   WHERE Id=@Id"
    ) {
        { "@Id", System.Data.DbType.Int32 },
    }.Build();
```

Statically building commands and requiring parameters prevents SQL Injection. Once you have the
`CommandBuilder` set up, you can then build and run the command using fairly standard ADO.Net.

```cs
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
```

It is recommended that a `DbProviderFactory` is used; this is the base class that can be
used in ADO.Net to assemble connections, commands, and many more.

```cs
private readonly DbProviderFactory dbProvider;
private readonly string connectionString;

public EmailManager(DbProviderFactory dbProvider, string connectionString)
{
    this.dbProvider = dbProvider;
    this.connectionString = connectionString;
}
```

It is important to use only the classes in `System.Data.Common` whenever possible; many of the
other classes in ADO.Net are `sealed`. When it is necessary, types should be tested prior to use
to not prevent testing.

## DeKreyConsulting.AdoTestability.Testing.Stubs

Provides fakes and classes that can be more easily overridden, mocked, or stubbed in tests.

## DeKreyConsulting.AdoTestability.Testing.SqlServer

Provides execution plan testing for SqlServer. This allows you to unit test the
`CommandBuilder.CommandText` property by actually sending it to the database and running an explain
plan. You can verify that the command is either a single result or multiple results and tests that
proper indexes are used.

```cs
builder.ExplainSingleResult(BuildSqlConnection());
builder.ExplainMultipleResult(BuildSqlConnection());
```

## DeKreyConsulting.AdoTestability.Testing.Moq

Provides classes that use the [Moq mocking framework](https://github.com/moq/moq4). This provides
easy mock setup, tracking of command executions, including parameter values.

```cs
[Fact]
public async Task OptOutByEmail(Func<EmailManager, string, Task<int>> optOut)
{
    const string email = "test@example.com";
    const int expectedResults = 2;
    var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { EmailManager.OptOutCommand, (mockCmd, record) => mockCmd.Setup(cmd => cmd.ExecuteNonQueryAsync(AnyCancellationToken)).ReturnsWithDelay(expectedResults).Callback(record) }, });

    var actualResults = (await CreateEmailManager(mocks.ProviderFactory.Object)).OptOutByEmailAsync(email);

    mocks.Connection.VerifySet(conn => conn.ConnectionString = connectionString);
    mocks.Commands[EmailManager.OptOutCommand].Verify(command => command.ExecuteNonQueryAsync(AnyCancellationToken), Times.Once());

    Assert.Equal(1, mocks.Executions[EmailManager.OptOutCommand].Count);
    Assert.Equal(mocks.Executions[EmailManager.OptOutCommand][0]["@Email"], email);

    Assert.Equal(expectedResults, actualResults);
}
```

# Contributing

Feel free to add an issue. Once there's an issue, feel free to provide a Pull Request. When doing
so, make sure to take into account testability and usability. However, you can also provide your
own packages that build upon the ones provided here.

# License

This is released under the MIT license.
