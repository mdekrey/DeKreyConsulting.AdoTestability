using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DeKreyConsulting.AdoTestability.Testing.Stubs
{
    public class FakeDbParameterCollection : DbParameterCollection
    {
        private List<DbParameter> list;

        private List<DbParameter> InnerList
        {
            get
            {
                List<DbParameter> list = this.list;
                if (list == null)
                {
                    list = new List<DbParameter>();
                    this.list = list;
                }
                return list;
            }
        }

        public override int Add(object value)
        {
            this.ValidateType(value);
            this.InnerList.Add((DbParameter)value);
            return this.Count - 1;
        }

        public override void AddRange(Array values)
        {
            values.OfType<object>().ToList().ForEach(obj => Add(obj));
        }

        public override void Clear()
        {
            InnerList.Clear();
        }

        public override bool Contains(string value)
        {
            return -1 != this.IndexOf(value);
        }

        public override bool Contains(object value)
        {
            return -1 != this.IndexOf(value);
        }

        public override void CopyTo(Array array, int index)
        {
            ((ICollection)this.InnerList).CopyTo(array, index);
        }

        public override int Count
        {
            get
            {
                if (this.list == null)
                {
                    return 0;
                }
                return this.list.Count;
            }
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.InnerList).GetEnumerator();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            int num = this.IndexOf(parameterName);
            return this.InnerList[num];
        }

        protected override DbParameter GetParameter(int index)
        {
            return this.InnerList[index];
        }

        public override int IndexOf(string parameterName)
        {
            return InnerList.FindIndex(p => p.ParameterName == parameterName);
        }

        public override int IndexOf(object value)
        {
            return InnerList.FindIndex(p => p == value);
        }

        public override void Insert(int index, object value)
        {
            this.ValidateType(value);
            this.InnerList.Insert(index, (DbParameter)value);
        }

        public override void Remove(object value)
        {
            InnerList.RemoveAt(this.IndexOf(value));
        }

        public override void RemoveAt(string parameterName)
        {
            InnerList.RemoveAt(this.IndexOf(parameterName));
        }

        public override void RemoveAt(int index)
        {
            InnerList.RemoveAt(index);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            int num = this.IndexOf(parameterName);
            value.ParameterName = parameterName;
            SetParameter(num, value);
        }

        protected override void SetParameter(int index, DbParameter newValue)
        {
            var innerList = this.InnerList;
            this.ValidateType(newValue);
            DbParameter oleDbParameter = innerList[index];
            innerList[index] = (DbParameter)newValue;
        }

        public override object SyncRoot
        {
            get
            {
                return ((ICollection)this.InnerList).SyncRoot;
            }
        }

#if !DOTNET5_4
        public override bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public override bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
#endif

        private void ValidateType(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (!typeof(DbParameter).IsInstanceOfType(value))
            {
                throw new ArgumentException();
            }
        }

    }
}
