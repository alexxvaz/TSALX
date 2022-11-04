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
}
