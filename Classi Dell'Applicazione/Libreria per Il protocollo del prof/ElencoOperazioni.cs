using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProtocolloGenerale
{
    public class ElencoOperazioni : Risposta
    {
        public ElencoOperazioni(string mex, OperazionePossibile[] movPoss) : base(mex)
        {
            MovimentiDisponibili = movPoss;
        }

        public OperazionePossibile[] MovimentiDisponibili { get; private set; }
    }
}
