using OFD.Data;
using OFD.Caching;
using OFD.Reflection;
using OFD.Properties;
using System.Collections.Generic;
using System;

namespace OFD
{
    /// <summary>
    /// This class is to be inherited for simple Oracle persistence provided by the OracleForDummies library. 
    /// </summary>
    public abstract class Model : IModel, ICloneable
    {
        // Properties.
        public int ID { get; set; }

        // Constructor.
        public Model(int id = 0)
        {
            if (id > 0)
            {
                Transactor.ScalarWhereCondition(this, string.Format(Resources.WhereID, id));
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
            Transactor.ScalarWhereCondition(this, string.Format(Resources.WhereID, id));
        }

        public virtual void SetWhereCondition(string condition)
        {
            Transactor.ScalarWhereCondition(this, condition);
        }

        public virtual void Drop()
        {
            Transactor.Drop(this);
        }

        // Static members.
        public static T ScalarWhereID<T>(int id) where T : Model, new()
        {
            T child = Reflector.GetUninitializedObject<T>();
            Transactor.ScalarWhereCondition(child, string.Format(Resources.WhereID, id));

            return child;
        }

        public static T ScalarWhereCondition<T>(string condition) where T : Model, new()
        {
            T child = Reflector.GetUninitializedObject<T>();
            Transactor.ScalarWhereCondition(child, condition);

            return child;
        }

        public static List<T> GetWhereCondition<T>(string condition) where T : Model, new()
        {     
            return Transactor.GetWhereCondition<T>(condition);
        }
    }
}