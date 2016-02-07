using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace OFD.Transact
{
    public struct Parameter
    {
        private static Dictionary<Type, OracleDbType> DbTypeMap
        {
            get
            {
                Dictionary<Type, OracleDbType> map = new Dictionary<Type, OracleDbType>();
                map.Add(typeof(bool), OracleDbType.Int16);
                map.Add(typeof(string), OracleDbType.Varchar2);
                map.Add(typeof(short), OracleDbType.Int16);
                map.Add(typeof(int), OracleDbType.Int32);
                map.Add(typeof(long), OracleDbType.Int64);
                map.Add(typeof(DateTime), OracleDbType.Date);

                return map;
            }
        }
        public string Name;
        public OracleDbType Type;
        public object Value;

        public Parameter(string name, Type type, object value)
        {
            this.Name = name;
            this.Type = DbTypeMap[type];
            this.Value = value;
        }
    }
}