using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolloGenerale
{
    public class Utente
    {
        public Utente(string id, string password, int ruolo)
        {
            IDUtente = id;
            Password = password;
            Ruolo = ruolo;
        }

        public string IDUtente { get; set; }

        public string Password { get; set; }

        public int Ruolo { get; set; }

    }
}
