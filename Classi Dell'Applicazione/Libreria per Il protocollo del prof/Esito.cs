using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolloGenerale
{
    public class Esito
    {
        public Esito(bool esitoOperazione, string messaggio)
        {
            EsitoOperazione = esitoOperazione;
            Message = messaggio;
        }

        public bool EsitoOperazione { get; private set; }

        public string Message { get; set; }
    }
}
