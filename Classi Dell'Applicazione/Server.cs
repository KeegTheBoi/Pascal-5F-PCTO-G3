using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GestioneDatabases;
using ProtocolloGenerale;
using System.Windows.Controls;

namespace ServerGestioneMagazzino
{
    class Server
    {
        public ManualResetEvent ManualReset = new ManualResetEvent(false);

        public Server(int port, IPAddress ip)
        {
            EndPoint = new IPEndPoint(ip, port);
            IndirizzoIP = ip;
        }

        public IPEndPoint EndPoint { get; set; }

        public IPAddress IndirizzoIP { get; set; }

        //Socket del server
        public Socket Ascoltatore { get; set; }

        public SqlDatabase BancaDati { private get; set; }

        public string FeedBack { get; private set; }

        public Client CurrentClient {get; set;}


        /// <summary>
        /// Inizializza il socket del server
        /// </summary>
        public void Initialize()
        {
            Ascoltatore = new Socket(IndirizzoIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Ascoltatore.Bind(EndPoint);
           
        }

        /// <summary>
        /// //Il server si mette ad ascoltare 
        /// </summary>
        public void StartListening()
        {
            //Il server si mette ad ascoltare 
            Ascoltatore.Listen(10);
        }

        /// <summary>
        /// Il socket del server accetta una connessione dal client
        /// </summary>
        public Client Accept()
        {
            Socket handler = Ascoltatore.Accept();
            Client c = new Client(handler);
            return c;
        }


        /// <summary>
        /// Chiude il server
        /// </summary>
        public void EndListening()
        {
            try
            {
                Ascoltatore.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                Ascoltatore.Close();
            }
        }

        public string Ricezione(Client c)
        {
            c.Ricezione();
            return c.Messaggio;
        }

        public void Risposta(string message, Client c)
        {
            c.Handler.Send(Encoding.ASCII.GetBytes(message));
        }



    }
}
