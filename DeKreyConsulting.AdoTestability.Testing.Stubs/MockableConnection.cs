using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DeKreyConsulting.AdoTestability.Testing.Stubs
{
    public abstract class MockableConnection : DbConnection
    {
        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return PublicBeginDbTransaction(isolationLevel);
        }

        public abstract DbTransaction PublicBeginDbTransaction(System.Data.IsolationLevel isolationLevel);

        protected override DbCommand CreateDbCommand()
        {
            return PublicCreateDbCommand();
        }

        public abstract DbCommand PublicCreateDbCommand();
    }
}
