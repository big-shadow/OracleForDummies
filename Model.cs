using OFD.Data;

namespace OFD
{
    /// <summary>
    /// This class is to be inherited for simple Oracle persistance provided by the OracleForDummies library. 
    /// </summary>
    public abstract class Model
    {
        public int ID { get; set; }

        protected Model(int id)
        {
            this.ID = id;

            if(this.ID > 0)
            {
                // TODO: Query for this record and assign it's properties. 
            }
        }

        public virtual void Save()
        {
            Transactor.Persist(this);
        }
    }
}