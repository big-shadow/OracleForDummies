using OFD.Transact;
using OFD.Properties;
using System.Collections.Generic;
using System;
using Oracle.ManagedDataAccess.Client;

namespace OFD
{
    /// <summary>
    /// This class is to be inherited for simple Oracle persistence provided by the OracleForDummies library. 
    /// </summary>
    public abstract class Model : ICloneable
    {
        // Properties.
        public int ID { get; set; }

        // Constructor.
        public Model(int id = 0)
        {
            if (id > 0)
            {
                Transactor.SetScalarWhere(this, string.Format(Resources.WhereID, id));
            }
        }

        // Interface implementations.
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        // Virtual members.
        public virtual void Save()
        {
            Transactor.Persist(this);
        }

        public virtual void SetWhereID(int id)
        {
            Transactor.SetScalarWhere(this, string.Format(Resources.WhereID, id));
        }

        public virtual void SetWhere(string condition)
        {
            Transactor.SetScalarWhere(this, condition);
        }

        // Static members.
        public static T ScalarWhereID<T>(int id) where T : Model, new()
        {
            return Transactor.GetScalarWhere<T>(string.Format(Resources.WhereID, id));
        }

        public static T ScalarWhere<T>(string condition) where T : Model, new()
        {
            return Transactor.GetScalarWhere<T>(condition);
        }

        public static List<T> GetWhere<T>(string condition) where T : Model, new()
        {
            return Transactor.GetWhere<T>(condition);
        }

        public static void DeleteWhere<T>(string condition) where T : Model, new()
        {
            Transactor.DeleteWhere<T>(condition);
        }

        public static T StoredProcedure<T>() where T : Model, new()
        {
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("p_id", OracleDbType.Int16, 1));
           
            return Transactor.StoredProcedure<T>(parameters)[0];
        }
    }
}