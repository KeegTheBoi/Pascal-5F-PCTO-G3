using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerGestioneMagazzino
{
    class Client
    {
        byte[] _buffer;
        public Client(Socket sckClient)
        {
            Handler = sckClient;
        }

        public Socket Handler { get; set; }

        public string Messaggio { get; set; }

        public void Ricezione()
        {
             _buffer = new byte[1024];
            string message = "";
           
            if (Handler.Connected)
            {
                int bufferSize = Handler.Receive(_buffer);
                message = Encoding.ASCII.GetString(_buffer, 0, bufferSize);
            }
            Messaggio = message;
            
        }
    }
}
