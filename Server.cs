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

namespace ServerGestioneMagazzino
{
    class Server
    {
        bool _onProcess = true;
        List<Thread> _listaClientThread;
        public Server(int port, IPAddress ip)
        {
            EndPoint = new IPEndPoint(ip, port);
            IndirizzoIP = ip;
        }

        public IPEndPoint EndPoint { get; set; }

        public IPAddress IndirizzoIP { get; set; }

        //Socket del seerver
        public Socket Ascoltatore { get; set; }


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
            _listaClientThread = new List<Thread>();
            //parte un thread dove viene gestita la ricezione
            Accept();
        }

        /// <summary>
        /// Il socket del server accetta una connessione dal client
        /// </summary>
        private void Accept()
        {
            //Esegue il ciclo mentre il server è attivo
            while (_onProcess)
            {                
                Socket clientSock = Ascoltatore.Accept();
                Client client = new Client(clientSock);
                client.Ricezione();
                Ricezione(client);
            }
        }

        /// <summary>
        /// Chiude il server
        /// </summary>
        public void EndListening()
        {
            foreach(Thread t in _listaClientThread)
            {
                t.Abort();
                t.Join();
            }
        }

        public void Ricezione(Client c)
        {
            string fileJSON = c.Messaggio;
            Prodotto ogg = JsonConvert.DeserializeObject<Prodotto>(fileJSON);


            SqlConnection connDb = new SqlConnection();
            SqlCommand sql = new SqlCommand();

            string riga;

            connDb.ConnectionString = "Data Source =(LocalDB)\\MSSQLLocalDB;"
                + " AttachDbFileName = |DataDirectory|magazzino.mdf";

        }



    }
}
