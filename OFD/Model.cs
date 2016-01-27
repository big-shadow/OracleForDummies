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
            GetWhereCondition("ID =" + id);
        }

        public virtual void GetWhereCondition(string condition)
        {
            Transactor.GetWhereCondition(this, condition);
        }

        public virtual void Drop()
        {
            Transactor.Drop(this);
        }
    }
}