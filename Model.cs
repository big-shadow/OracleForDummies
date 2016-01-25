using OFD.Data;

namespace OFD
{
    /// <summary>
    /// This class is to be inherited for simple Oracle persistance provided by the OracleForDummies library. 
    /// </summary>
    public class Model
    {
        public int ID { get; set; }

        public Model(int id = 0)
        {
            if(id > 0)
            {
                GetWhereID(id);
            }
        }

        public virtual void Save()
        {
            Transactor.Persist(this);
        }

        public virtual void GetWhereID(int id)
        {      
            Transactor.GetWhereID(this, id);
        }

        public static T GetWhereID<T>(int id) where T : new()
        {
            return Transactor.GetWhereID<T>(typeof(T), id);
        }
    }
}