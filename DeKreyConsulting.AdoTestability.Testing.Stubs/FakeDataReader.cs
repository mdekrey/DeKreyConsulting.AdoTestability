using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeKreyConsulting.AdoTestability.Testing.Stubs
{
    public class FakeDataReader : DbDataReader
    {
        private int resultSet = 0;
        private int index = -1;
        private Dictionary<string, object>[][] data;
        private bool withDelay;

        public FakeDataReader(params Dictionary<string, object>[][] data)
        {
            this.data = data;
        }

        public bool WithDelay
        {
            get { return withDelay; }
            set { withDelay = value; }
        }

        public override int Depth
        {
            get { return 0; }
        }

        public override int FieldCount
        {
            get { return data[resultSet][index].Count; }
        }

        public override bool GetBoolean(int ordinal) =>
            Convert.ToBoolean(GetValue(ordinal));
        
        public override byte GetByte(int ordinal) =>
            Convert.ToByte(GetValue(ordinal));

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            Array.Copy(((byte[])GetValue(ordinal)), (int)dataOffset, buffer, bufferOffset, length);
            return length;
        }

        public override char GetChar(int ordinal) =>
            Convert.ToChar(GetValue(ordinal));

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            Array.Copy(((char[])GetValue(ordinal)), (int)dataOffset, buffer, bufferOffset, length);
            return length;
        }

        public override string GetDataTypeName(int ordinal) =>
            GetFieldType(ordinal).FullName;

        public override DateTime GetDateTime(int ordinal) =>
            Convert.ToDateTime(GetValue(ordinal));

        public override decimal GetDecimal(int ordinal) =>
            Convert.ToDecimal(GetValue(ordinal));

        public override double GetDouble(int ordinal) =>
            Convert.ToDouble(GetValue(ordinal));

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException("This use of DataReader is rare enough that I didn't know it existed until I started implementing this. I don't think it'll be necessary. If you use it, open an issue, and sorry in advance!");
        }

        public override Type GetFieldType(int ordinal) =>
            GetValue(ordinal).GetType();

        public override float GetFloat(int ordinal) =>
            Convert.ToSingle(GetValue(ordinal));

        public override Guid GetGuid(int ordinal) =>
            Guid.Parse(Convert.ToString(GetValue(ordinal)));

        public override short GetInt16(int ordinal) =>
            Convert.ToInt16(GetValue(ordinal));

        public override int GetInt32(int ordinal) =>
            Convert.ToInt32(GetValue(ordinal));

        public override long GetInt64(int ordinal) =>
            Convert.ToInt64(GetValue(ordinal));

        public override string GetName(int ordinal) =>
            data[resultSet][index].Keys.ToArray()[ordinal];

        public override int GetOrdinal(string name) =>
            data[resultSet][index].Keys.ToList().IndexOf(name);

        public override string GetString(int ordinal) =>
            Convert.ToString(GetValue(ordinal));

        public override object GetValue(int ordinal) =>
            data[resultSet][index][GetName(ordinal)];

        public override int GetValues(object[] values)
        {
            data[resultSet][index].Values.CopyTo(values, 0);
            return data[resultSet][index].Count;
        }

        public override bool HasRows
        {
            get { return data[resultSet].Length > 0; }
        }

        public override bool IsClosed
        {
            get { return false; }
        }

        public override bool IsDBNull(int ordinal) =>
            GetValue(ordinal) is DBNull;

        public override bool NextResult()
        {
            resultSet++;
            index = -1;
            return resultSet < data.Length;
        }

        public override bool Read()
        {
            index++;
            return index < data[resultSet].Length;
        }

        public override Task<bool> ReadAsync(System.Threading.CancellationToken cancellationToken)
        {
            if (withDelay)
                return Task.Delay(10).ContinueWith(t => base.ReadAsync(cancellationToken)).Unwrap();
            return base.ReadAsync(cancellationToken);
        }
#if !DOTNET5_4
        public override void Close()
        {
        }

        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException("Copying from a DataReader to a DataTable is quite ineffient and DataTable is even more difficult to test than DataReader. Sorry I didn't implement it, but good luck!");
        }
#endif

        public override int RecordsAffected => 0;

        public override object this[string name]
        {
            get { return data[resultSet][index][name]; }
        }

        public override object this[int ordinal] =>
            GetValue(ordinal);
    }
}
