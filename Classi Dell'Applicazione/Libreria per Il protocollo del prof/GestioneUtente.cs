﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolloGenerale
{
    public class GestioneUtente
    {
        public GestioneUtente(string codUtente, string nomeUtente, string password, string ruolo)
        {
            IDUtente = codUtente;
            Nome = nomeUtente;
            Password = password;
            Ruolo = ruolo;
        }

        string IDUtente { get; set; }

        string Nome { get; set; }

        public string Password {get; set;}

        string Ruolo { get; set; }


    }
}
