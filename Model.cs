using OFD.Data;

namespace OFD
{
    /// <summary>
    /// This class is to be inherited for simple Oracle persistance provided by the OracleForDummies library. 
    /// </summary>
    public abstract class Model
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Model(int id = -1)
        {
            this.ID = id;
            this.Name = "Big Ray";
        }

        public virtual void Save()
        {
            Transactor.Persist(this);
        }

    }
}