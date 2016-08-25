using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using DeKreyConsulting.AdoTestability.Testing.Stubs;
using DeKreyConsulting.AdoTestability.Testing.Moq;
using DeKreyConsulting.AdoTestability;

namespace Moq
{
    public delegate void SetupCommandBuilderMock(Mock<MockableCommand> mockCommand, Action recordExecution);

    [ExcludeFromCodeCoverage]
    public static class CommandBuilderMoqExtensions
    {
        public static CommandBuilderMocks SetupFor(this Mock<DbProviderFactory> mockFactory, Dictionary<CommandBuilder, SetupCommandBuilderMock> commandBuilders, bool withStandardDelay = false)
        {
            var mockConnection = new Mock<MockableConnection>();
            mockFactory.Setup(factory => factory.CreateConnection()).Returns(mockConnection.Object);
            var temp = mockConnection.SetupFor(commandBuilders, withStandardDelay);
            return new CommandBuilderMocks(mockFactory, temp.Connection, temp.Commands, temp.Executions);
        }

        public static CommandBuilderMocks SetupFor(this Mock<MockableConnection> mockConnection, Dictionary<CommandBuilder, SetupCommandBuilderMock> commandBuilders, bool withStandardDelay = false)
        {
            mockConnection.CallBase = true;
            var commandBuilderLookup = commandBuilders.ToDictionary(builder => builder.Key.CommandText, builder => new { Builder = builder.Key, Setup = builder.Value });
            var commands = new Dictionary<CommandBuilder, Mock<MockableCommand>>();
            var executions = new Dictionary<CommandBuilder, List<IReadOnlyDictionary<string, object>>>();

            if (withStandardDelay)
            {
                mockConnection.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>())).ReturnsWithDelay();
            }

            mockConnection.Setup(conn => conn.PublicCreateDbCommand()).Returns(() =>
            {
                var calls = new List<IReadOnlyDictionary<string, object>>();
                var mockCommand = new Mock<MockableCommand> { DefaultValue = DefaultValue.Mock, CallBase = true };
                Action record = () => RegisterExecutions(mockCommand.Object, calls);
                mockCommand.Object.OnExecute += record;
                mockCommand.SetupGet(cmd => cmd.PublicDbParameterCollection).Returns(new FakeDbParameterCollection());
                mockCommand.SetupSet(cmd => cmd.CommandText = It.IsAny<string>()).Callback<string>(commandText =>
                {
                    if (withStandardDelay)
                    {
                        mockCommand.Setup(cmd => cmd.PublicExecuteDbDataReaderAsync(It.IsAny<CommandBehavior>(), It.IsAny<CancellationToken>()))
                            .ReturnsWithDelay<MockableCommand, DbDataReader, CommandBehavior, CancellationToken>((cb, ct) => mockCommand.Object.PublicExecuteDbDataReader(cb));
                        mockCommand.Setup(cmd => cmd.ExecuteScalarAsync(It.IsAny<CancellationToken>()))
                            .ReturnsWithDelay(() => mockCommand.Object.ExecuteScalar());
                        mockCommand.Setup(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
                            .ReturnsWithDelay(() => mockCommand.Object.ExecuteNonQuery());
                    }

                    var builder = commandBuilderLookup[commandText];
                    builder.Setup?.Invoke(mockCommand, record);
                    commands[builder.Builder] = mockCommand;
                    executions[builder.Builder] = calls;
                });

                mockCommand.Setup(cmd => cmd.PublicCreateDbParameter()).Returns(() =>
                {
                    var mockParam = new Mock<DbParameter>();
                    mockParam.SetupAllProperties();

                    return mockParam.Object;
                });

                return mockCommand.Object;
            });

            return new CommandBuilderMocks(
                providerFactory: null,
                connection: mockConnection,
                commands: commands,
                executions: executions);
        }

        private static void RegisterExecutions(DbCommand dbCommand, List<IReadOnlyDictionary<string, object>> calls)
        {
            var captured = new Dictionary<string, object>();
            foreach (var param in dbCommand.Parameters.OfType<DbParameter>())
            {
                if (param.Value == null)
                {
                    throw new InvalidOperationException($"Parameter not allowed to be Null; at least set to DBNull. Parameter: {param.ParameterName}");
                }
                captured[param.ParameterName] = param.Value;
            }
            calls.Add(new ReadOnlyDictionary<string, object>(captured));
        }
    }
}
