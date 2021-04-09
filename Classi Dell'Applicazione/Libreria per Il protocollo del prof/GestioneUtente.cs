using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolloGenerale
{
    public class GestioneUtente
    {
        public GestioneUtente(string codUtente, string nomeUtente, string password, int ruolo)
        {
            IDUtente = codUtente;
            Nome = nomeUtente;
            Password = password;
            Ruolo = ruolo;
        }

        public string IDUtente { get; set; }

        public string Nome { get; set; }

        public string Password {get; set;}

        public int Ruolo { get; set; }


    }
}
