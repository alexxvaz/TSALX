using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportarTSALX
{
    public class Liga
    {
        public int IDLiga { get; set; }
        public string Nome { get; set; }
        public string NomePais { get; set; }
    }
    public class Bandeira
    {
        public string Sigla { get; set; }
        public string NomePais { get; set; }
    }
    public class Equipe
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string NomePais { get; set; }
        public bool Selecao { get; set; }
    }
    public class CampeonatoBKP
    {
        public int ID { get; set; }
        public int IDRegiao { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public bool Selecao { get; set; }
    }

    public class Temporada
    {
        public int ID { get; set; }
        public short AnoInicial { get; set; }
        public short AnoFinal { get; set; }
    }

}
