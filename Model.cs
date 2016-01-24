using System;
using OFD.Data;

namespace OFD
{
    public abstract class Model
    {
        public int ID { get; set; }

        public Model(int id = -1)
        {

        }

        public virtual void Save()
        {          
            Transactor.Execute("select 1 from dual");    
        }
           
    }
}