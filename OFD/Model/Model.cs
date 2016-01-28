using OFD.Data;
using OFD.Caching;
using OFD.Reflection;
using OFD.Properties;

namespace OFD
{
    /// <summary>
    /// This class is to be inherited for simple Oracle persistence provided by the OracleForDummies library. 
    /// </summary>
    public abstract class Model : IModel
    {
        // Properties.
        public int ID { get; set; }
        public Cache Cache { get; set; }

        // Constructor.
        public Model(int id = 0)
        {
            this.Cache = new Cache(); 
            this.Cache.IdentityCache = Reflector.GetIdentityMap(this.GetType());

            if (id > 0)
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
            T child = Reflector.GetUninitializedObject<T>();
            Transactor.GetWhereCondition(child, string.Format(Resources.WhereID, id));

            return child;
        }

        public static T GetWhereCondition<T>(string condition) where T : Model, new()
        {
            T child = Reflector.GetUninitializedObject<T>();
            Transactor.GetWhereCondition(child, condition);

            return child;
        }
    }
}