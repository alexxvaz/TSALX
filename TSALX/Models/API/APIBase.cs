using System;

namespace TSALX.Models.API
{
    public abstract class APIBase
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string NomePais { get; set; }
        public virtual string Escudo { get { throw new NotImplementedException( "Deve implementar a rota do logo" ); } }
    }
}