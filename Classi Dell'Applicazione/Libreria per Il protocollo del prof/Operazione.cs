using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolloGenerale
{
    public class Operazione
    {
        public Operazione(string codProd, string nome, int quantita, int codOperazione)
        {
            IDProdotto = codProd;
            QuantitaProdotti = quantita;
            CodOperazione = codOperazione;
        }

        public string IDProdotto { get; private set; }
        public string NomeProdotto { get; set; }
        public int QuantitaProdotti { get; private set; }
        public int CodOperazione { get; private set; }
    }
}
