using OFD.Data;
using OFD.Properties;
using System;

namespace OFD
{
    /// <summary>
    /// This class is to be inherited for simple Oracle persistence provided by the OracleForDummies library. 
    /// </summary>
    public abstract class Model : IModel
    {
        public int ID { get; set; }

        public Model(int id = 0)
        {
            if(id > 0)
            {
                Transactor.GetWhereCondition(this, string.Format(Resources.WhereID, id));
            }
        }

        // Virtual members.
        public virtual void Save()
        {
            Transactor.Persist(this);
        }

        public virtual void SetWhereID(int id)
        {
            Transactor.GetWhereCondition(this, string.Format(Resources.WhereID, id));
        }

        public virtual void SetWhereCondition(string condition)
        {
            Transactor.GetWhereCondition(this, condition);
        }

        public virtual void Drop()
        {
            Transactor.Drop(this);
        }

        // Static members.
        public static T GetWhereID<T>(int id) where T : Model, new()
        {
            T child = new T();
            Transactor.GetWhereCondition(child, string.Format(Resources.WhereID, id));

            return child;
        }

        public static T GetWhereCondition<T>(string condition) where T : Model, new()
        {
            T child = new T();
            Transactor.GetWhereCondition(child, condition);

            return child;
        }
    }
}