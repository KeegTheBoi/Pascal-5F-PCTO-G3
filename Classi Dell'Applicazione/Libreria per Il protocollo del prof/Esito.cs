using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolloGenerale
{
    public class Esito : Risposta
    {
        public Esito(bool esitoOperazione, string messaggio) : base(messaggio)
        {
            EsitoOperazione = esitoOperazione;
        }

        public bool EsitoOperazione { get; private set; }
    }
}
