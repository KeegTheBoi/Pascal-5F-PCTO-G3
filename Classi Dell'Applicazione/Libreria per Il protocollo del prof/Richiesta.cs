using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolloGenerale
{
    public class Richiesta
    {
        // Proprietà

        public string Utente { get; set; }

        public string Password { get; set; }

        public int CodiceOperazione { get; set; }

        public int CodiceRichiesta { get; set; }

        public int Quantita { get; set; }

        public string CodiceProdotto { get; set; }

        public string NomeProdotto { get; set; }


    }
}
