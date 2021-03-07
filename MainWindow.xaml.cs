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
using GestioneDatabases;

namespace ServerGestioneMagazzino
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml     
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlDatabase database;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Il dns ottiene il nome dell'host della macchine in locale
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[5];
            //ottiene l'indirizzo ip della macchina e la sua relativa port  
            
            //prove
            string utenti = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(@"jsonfile.txt"))
            {
                utenti = sr.ReadToEnd();
            }

            string operazioni = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(@"operazioneProva.txt"))
            {
                operazioni = sr.ReadToEnd();
            }

            //da mettere in un metodo
            ProtocolloComune prot = new ProtocolloComune(database);
            prot.MexJson = utenti;
            prot.ConversioneMessaggio();
            txtIterazione.Text = prot._feedback;
            prot.InstauraConnessioneDatabase();
            txtIterazione.Text = prot._feedback;

            

            prot.MexJson = operazioni;
            prot.ConversioneMessaggio();
            txtIterazione.Text += "\n----------------------------";
            txtIterazione.Text += prot._feedback;
            prot.InstauraConnessioneDatabase();
            txtIterazione.Text += prot._feedback;
            txtIterazione.ScrollToEnd();

            //per visualizzare le operazioni in una tabella
            dtaOperazioneEffettuate.DataContext = prot.GetTableOperazioni().DefaultView;

            //Server s = new Server(22, ipAddress);
            //s.Initialize();
            //s.StartListening();


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Inizializzazione degli elementi presenti nelle combobox
            //Locazione dove si possono aggiungere degli elementi
            string[] autenticazioni = { "", "Autenticazione Windows", "Autenticazione SQL" };
            string[] nomiPC = { "PCFALCAO", "PC1232" };
            cmbAutenticazione.ItemsSource = autenticazioni;
            cmbUtentiSQL.ItemsSource = nomiPC;
        }

        private void cmbAutenticazione_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAutenticazione.SelectedItem == "Autenticazione SQL")
            {
                cmbUtentiSQL.IsEnabled = true;
                pssSQL.IsEnabled = true;
            }
            else if (cmbAutenticazione.SelectedItem == "Autenticazione Windows")
            {
                cmbUtentiSQL.IsEnabled = true;
                pssSQL.IsEnabled = false;
            }
        }

        private void cmbUtentiSQL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnConnetti_Click(object sender, RoutedEventArgs e)
        {
            database = new SqlDatabase(cmbAutenticazione.Text);
            if (database.ConnettiDatabase(cmbUtentiSQL.Text, "Magazzino", "sa", pssSQL.Password))
            {
                try
                {
                    grpAutenticazione.IsEnabled = false;
                    btnStart.IsEnabled = true;
                    MessageBox.Show("Connessione avvenuta con successo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Tentativo di Connessione fallita\nErrore: " + ex.Message);
                }

            }
            else
                MessageBox.Show("Autenticazione Insesistente");
        }
    }   
}
