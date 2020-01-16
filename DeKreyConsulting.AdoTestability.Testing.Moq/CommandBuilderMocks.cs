using DeKreyConsulting.AdoTestability.Testing.Stubs;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DeKreyConsulting.AdoTestability.Testing.Moq
{
#if NETFRAMEWORK
    [ExcludeFromCodeCoverage]
#endif
    public class CommandBuilderMocks
    {
        public CommandBuilderMocks(
            Mock<DbProviderFactory> providerFactory,
            Mock<MockableConnection> connection,
            Dictionary<CommandBuilder, Mock<MockableCommand>> commands,
            Dictionary<CommandBuilder, List<IReadOnlyDictionary<string, object>>> executions)
        {
            this.ProviderFactory = providerFactory;
            this.Connection = connection;
            this.Commands = commands;
            this.Executions = executions;
        }

        public Mock<DbProviderFactory> ProviderFactory { get; }

        public Mock<MockableConnection> Connection { get; }

        public Dictionary<CommandBuilder, Mock<MockableCommand>> Commands { get; }

        public Dictionary<CommandBuilder, List<IReadOnlyDictionary<string, object>>> Executions { get; }

        public static CommandBuilderMocks SetupFor(Dictionary<CommandBuilder, SetupCommandBuilderMock> commandBuilders, bool withStandardDelay = false)
        {
            return new Mock<DbProviderFactory>() { DefaultValue = DefaultValue.Mock }.SetupFor(commandBuilders, withStandardDelay);
        }
    }
}
