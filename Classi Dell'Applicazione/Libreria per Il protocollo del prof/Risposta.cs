using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolloGenerale
{
    public abstract class Risposta
    {
        public Risposta(string descrizione)
        {
            Messaggio = descrizione;
        }

        public string Messaggio { get; private set; }
    }
}
