using System;
using System.Collections.Generic;

namespace ImportarTSALX
{
    public class CompararPartida : IEqualityComparer<Aposta>
    {
        public bool Equals( Aposta x, Aposta y )
        {
            return x.PartidaID == y.PartidaID;
        }

        public int GetHashCode( Aposta obj )
        {
            return Convert.ToInt32( obj.PartidaID );
        }
    }
}
