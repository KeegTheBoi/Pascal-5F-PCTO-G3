using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerGestioneMagazzino
{
    class Risposta
    {
        public Risposta(Client c)
        {
            Cliente = c;
        }

        public Client Cliente { get; set; }
    }
}
