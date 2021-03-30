using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolloGenerale
{
    class GestioneProdotto : Operazione
    {
        public GestioneProdotto(int cod, string nome, int quantita, string codProd) : base (cod)
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
