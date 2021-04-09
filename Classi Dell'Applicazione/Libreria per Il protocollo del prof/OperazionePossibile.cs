using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolloGenerale
{
    public class OperazionePossibile
    {
        public OperazionePossibile(int codOp, bool? carico, string descri)
        {
            IDOperazione = codOp;
            Carico = carico;
            Descrizione = descri;
        }

        public int IDOperazione { get; private set; }
        public bool? Carico { get; private set; }
        public string Descrizione { get; private set; }
    }
}
