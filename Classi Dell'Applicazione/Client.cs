using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerGestioneMagazzino
{
    class Client
    {
        public Client(Socket sckClient)
        {
            Handler = sckClient;
        }

        public Socket Handler { get; set; }

        public string Messaggio { get; set; }

        public void Ricezione()
        {

            string messaggio = "";
            byte[] buffer = new byte[1024];
           
            if (Handler.Connected)
            {
                int nByte = Handler.Receive(buffer);
                messaggio = Encoding.ASCII.GetString(buffer, 0, nByte);
            }
            Messaggio = messaggio;
            
            
        }
    }
}
