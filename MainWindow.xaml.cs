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
using ProtocolloGenerale;

namespace ServerGestioneMagazzino
{
    /// <summary>
    ///  Keegan Carlo Falcao, Guglielmi Juri, Piscaglia Katia 5F    
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlDatabase database;
        public MainWindow()
        {
            InitializeComponent();
        }

        List<string> _nomiPC;

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //Non esegue il click se la comunicazione è gia aperta
            btnStart.IsEnabled = false;
            //Il dns ottiene il nome dell'host della macchine in locale
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[3];

            Server s = new Server(32000, ipAddress);
            s.Initialize();
            s.StartListening();
            lblStatus.Content = "Connesso";
            ProtocolloComune prot = new ProtocolloComune(database);
            bool connOpen = true;

            Task.Run(() =>
            {
                while (true)
                {
                    connOpen = true;
                    Client c = s.Accept();

                    while (connOpen)
                    {
                        prot.MexJson = s.Ricezione(c);
                        prot.ConversioneMessaggio();
                        prot.InstauraConnessioneDatabase();
                        if (prot._closed)
                            connOpen = false;

                        Dispatcher.Invoke(() =>
                        {
                            txtIterazione.Text += "\n--------------------------------------\n" + prot._feedback;

                            if (prot._successo)
                                dtaOperazioneEffettuate.DataContext = prot.GetTableOperazioni().DefaultView;
                            txtIterazione.ScrollToEnd();
                        });

                        string jsonFileInvio = JsonConvert.SerializeObject(prot.Risposta);
                        //to update
                        s.Risposta(jsonFileInvio, c);

                    }

                }
            });
            

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Inizializzazione degli elementi presenti nelle combobox
            //Locazione dove si possono aggiungere degli elementi
            string[] autenticazioni = { "", "Autenticazione Windows", "Autenticazione SQL" };
            _nomiPC = new List<string>(new string[] { "PCFALCAO", "PC1232" });
            cmbAutenticazione.ItemsSource = autenticazioni;
            cmbUtentiSQL.ItemsSource = _nomiPC;
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
                    database.IniziaConessione();
                    grpAutenticazione.IsEnabled = false;
                    btnStart.IsEnabled = true;
                    MessageBox.Show("Connessione avvenuta con successo");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Tentativo di Connessione fallita\nErrore: " + ex.Message);
                }
                finally
                {
                    database.ChiudiConnessione();
                }
            }
            else
                MessageBox.Show("Autenticazione Insesistente");
        }

        private void btnAggiungi_Click(object sender, RoutedEventArgs e)
        {            
            _nomiPC.Add(txtNuovoNomePC.Text);
            cmbUtentiSQL.ItemsSource = _nomiPC;
            cmbUtentiSQL.Items.Refresh();
        }

    }   
}
