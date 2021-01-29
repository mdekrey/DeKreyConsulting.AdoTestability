using DeKreyConsulting.AdoTestability.Testing.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Xunit;

namespace DeKreyConsulting.AdoTestability.Tests
{
    using CommandSetup = Dictionary<CommandBuilder, SetupCommandBuilderMock>;

    public class CommandBuilderShould
    {
        [Fact]
        public void ApplyOrderedParametersOnlyIfExactLengthMatch()
        {
            var builder =
                new CommandBuilderFactory(commandText: @"SELECT 1")
                {
                    { "@param", DbType.String },
                    { "@param2", DbType.String, p => p.Size = 128 }
                }.Build();
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { builder, (mockCmd, record) => { } }, });

            Assert.Throws<InvalidOperationException>(() => builder.BuildFrom(mocks.Connection.Object, new[] { "param" }));
            Assert.Throws<InvalidOperationException>(() => builder.BuildFrom(mocks.Connection.Object, new[] { "param", "param2", "param3" }));
        }

        [Fact]
        public void ApplyNamedParameters()
        {
            var builder =
                new CommandBuilderFactory(commandText: @"SELECT 1")
                {
                    { "@param", DbType.String },
                    { "@param2", DbType.String, p => p.Size = 128 }
                }.Build();
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { builder, (mockCmd, record) => { } }, });

            var command = builder.BuildFrom(mocks.Connection.Object, new Dictionary<string, object> { { "@param", "param" }, { "@param2", "param2" } });

            Assert.Collection(command.Parameters.OfType<DbParameter>(),
                param => { Assert.Equal("@param", param.ParameterName); Assert.Equal("param", param.Value); },
                param2 => { Assert.Equal("@param2", param2.ParameterName); Assert.Equal("param2", param2.Value); }
            );
        }


        [Fact]
        public void PartiallyApplyNamedParameters()
        {
            var builder =
                new CommandBuilderFactory(commandText: @"SELECT 1")
                {
                    { "@param", DbType.String },
                    { "@param2", DbType.String, p => p.Size = 128 }
                }.Build();
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { builder, (mockCmd, record) => { } }, });

            var command = builder.BuildFrom(mocks.Connection.Object, new Dictionary<string, object> { { "@param", "param" } });

            Assert.Collection(command.Parameters.OfType<DbParameter>(),
                param => { Assert.Equal("@param", param.ParameterName); Assert.Equal("param", param.Value); },
                param2 => { Assert.Equal("@param2", param2.ParameterName); Assert.Null(param2.Value); }
            );
        }
    }
}
