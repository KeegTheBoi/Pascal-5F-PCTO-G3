using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolloGenerale
{
    public class GestioneProdotto
    {
        public GestioneProdotto(string nome, int quantita, string codProd)
        {
            NomeProdotto = nome;
            QuantitaProdotti = quantita;
            IDProdotto = codProd;
        }

        public string IDProdotto { get; private set; }
        public string NomeProdotto { get; set; }
        public int QuantitaProdotti { get; private set; }
    }
}
