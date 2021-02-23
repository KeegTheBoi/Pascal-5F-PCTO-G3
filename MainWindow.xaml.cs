using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace ServerGestioneMagazzino
{
    /// <summary>
    /// Keegan Carlo Falcao 5F
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Il dns ottiene il nome dell'host della macchine in locale
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[2];
            //ottiene l'indirizzo ip della macchina e la sua relativa port           


            //Creazione della connessione
            SqlConnection connDb = new SqlConnection();
            SqlCommand sql = new SqlCommand();
            SqlDataReader reader;

            string riga;


            connDb.ConnectionString  = "Data Source=PC1232;" +
              "Initial Catalog=Magazzino;" +
              "User id=sa;" +
              "Password=burbero2020;";

            connDb.Open();


            string query = "SELECT * FROM Utenti";

            sql.CommandText = query;
            sql.Connection = connDb;

            reader = sql.ExecuteReader();

            string r = reader.GetString(0);


            Server s = new Server(22, ipAddress);
            s.Initialize();
            s.StartListening();
          
           
        }
    }
}
