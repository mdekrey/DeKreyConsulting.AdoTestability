using DeKreyConsulting.AdoTestability.Testing.Stubs;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace DeKreyConsulting.AdoTestability.Testing.Moq
{
    [ExcludeFromCodeCoverage]
    public class CommandBuilderMocks
    {
        public Mock<MockableConnection> Connection { get; set; }

        public Dictionary<CommandBuilder, Mock<MockableCommand>> Commands { get; set; }

        public Dictionary<CommandBuilder, List<IReadOnlyDictionary<string, object>>> Executions { get; set; }
    }
}
