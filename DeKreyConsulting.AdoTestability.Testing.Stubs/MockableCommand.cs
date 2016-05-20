using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DeKreyConsulting.AdoTestability.Testing.Stubs
{
    public abstract class MockableCommand : DbCommand
    {
        protected override DbConnection DbConnection
        {
            get
            {
                return PublicDbConnection;
            }
            set
            {
                PublicDbConnection = value;
            }
        }

        public abstract DbConnection PublicDbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return PublicDbParameterCollection; }
        }

        public abstract DbParameterCollection PublicDbParameterCollection { get; }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return PublicDbTransaction;
            }
            set
            {
                PublicDbTransaction = value;
            }
        }

        public abstract DbTransaction PublicDbTransaction { get; set; }

        protected override DbParameter CreateDbParameter()
        {
            return PublicCreateDbParameter();
        }

        public abstract DbParameter PublicCreateDbParameter();

        protected override System.Threading.Tasks.Task<DbDataReader> ExecuteDbDataReaderAsync(System.Data.CommandBehavior behavior, System.Threading.CancellationToken cancellationToken)
        {
            return PublicExecuteDbDataReaderAsync(behavior, cancellationToken);
        }

        public virtual System.Threading.Tasks.Task<DbDataReader> PublicExecuteDbDataReaderAsync(System.Data.CommandBehavior behavior, System.Threading.CancellationToken cancellationToken)
        {
            return base.ExecuteDbDataReaderAsync(behavior, cancellationToken);
        }

        protected override DbDataReader ExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            return PublicExecuteDbDataReader(behavior);
        }

        public virtual DbDataReader PublicExecuteDbDataReader(System.Data.CommandBehavior behavior)
        {
            var temp = OnExecute;
            if (temp != null)
            {
                OnExecute();
            }
            return null;
        }

        public override object ExecuteScalar()
        {
            var temp = OnExecute;
            if (temp != null)
            {
                OnExecute();
            }
            return null;
        }

        public override int ExecuteNonQuery()
        {
            var temp = OnExecute;
            if (temp != null)
            {
                OnExecute();
            }
            return 0;
        }

        public event Action OnExecute;
    }
}
