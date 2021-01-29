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

    public class CommandBuilderFactoryShould
    {
        [Fact]
        public void DisallowMixedNamedAndUnnamedParameters()
        {
            Assert.Throws<ArgumentException>(() => 
                new CommandBuilderFactory(commandText: @"SELECT 1")
                {
                    { "@param", DbType.String },
                    { DbType.String }
                }
            );
        }

        [Fact]
        public void AllowCustomizedNamedParameters()
        {
            var builder =
                new CommandBuilderFactory(commandText: @"SELECT 1")
                {
                    { "@param", DbType.String },
                    { "@param2", DbType.String, p => p.Size = 128 }
                }.Build();
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { builder, (mockCmd, record) => { } }, });

            var command = builder.BuildFrom(mocks.Connection.Object);

            Assert.Collection(command.Parameters.OfType<DbParameter>(),
                param => { Assert.Equal("@param", param.ParameterName); Assert.Equal(DbType.String, param.DbType); Assert.NotEqual(128, param.Size); },
                param2 => { Assert.Equal("@param2", param2.ParameterName); Assert.Equal(DbType.String, param2.DbType); Assert.Equal(128, param2.Size); }
            );
        }

        [Fact]
        public void AllowCustomizedUnnamedNamedParameters()
        {
            var builder =
                new CommandBuilderFactory(commandText: @"SELECT 1")
                {
                    { DbType.String },
                    { DbType.String, p => p.Size = 128 }
                }.Build();
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { builder, (mockCmd, record) => { } }, });

            var command = builder.BuildFrom(mocks.Connection.Object);

            Assert.Collection(command.Parameters.OfType<DbParameter>(),
                param => { Assert.Equal(DbType.String, param.DbType); Assert.NotEqual(128, param.Size); },
                param2 => { Assert.Equal(DbType.String, param2.DbType); Assert.Equal(128, param2.Size); }
            );
        }

        [Fact]
        public void ApplyOrderedParameters()
        {
            var builder =
                new CommandBuilderFactory(commandText: @"SELECT 1")
                {
                    { "@param", DbType.String },
                    { "@param2", DbType.String, p => p.Size = 128 }
                }.Build();
            var mocks = CommandBuilderMocks.SetupFor(new CommandSetup { { builder, (mockCmd, record) => { } }, });

            var command = builder.BuildFrom(mocks.Connection.Object, new[] { "param", "param2" });

            Assert.Collection(command.Parameters.OfType<DbParameter>(),
                param => { Assert.Equal("@param", param.ParameterName); Assert.Equal("param", param.Value); },
                param2 => { Assert.Equal("@param2", param2.ParameterName); Assert.Equal("param2", param2.Value); }
            );
        }
    }
}
