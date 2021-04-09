using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace GestioneDatabases
{
    public class SqlDatabase
    {
        string _tipoAutent;
        public SqlDatabase(string autentic)
        {
            Connessione = new SqlConnection();
            _tipoAutent = autentic;
        }

        public SqlConnection Connessione { get; private set; }

        public bool ConnettiDatabase(string nomePC, string nomeDatabase, string user, string pass)
        {
            string connString = $"Server = {nomePC}; Database = {nomeDatabase};";
            if (_tipoAutent == "Autenticazione SQL")
                connString += $"User Id = {user}; Password = {pass};";
            else if (_tipoAutent == "Autenticazione Windows")
                connString += "Trusted_Connection = True";
            else
                return false;
            Connessione.ConnectionString = connString;
            return true;
        }

        public void IniziaConessione()
        {
            Connessione.Open();
        }

        public void ChiudiConnessione()
        {
            Connessione.Close();
        }

        /// <summary>
        /// Crea, modifica ed elimina un record
        /// </summary>
        /// <param name="insertString"></param>
        public void CreateUpdateDeleteRecord(string cud)
        {
            //da modificare la gestione degli errore
            SqlCommand command = new SqlCommand(cud, Connessione);
            command.ExecuteNonQuery();
        }

        public DataTable VisualizzazioneTabella(string query)
        {
            DataTable tabella = new DataTable();
            SqlDataReader reader = LettoreComando(query);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                tabella.Columns.Add(reader.GetName(i));
            }

            while (reader.Read())
            {
                DataRow riga = tabella.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    riga[i] = reader.GetValue(i);
                }
                tabella.Rows.Add(riga);
            }

            reader.Close();
            return tabella;
        }


        public SqlDataReader LettoreComando(string query)
        {
            //Da mettere in un metodo a parte """"!""""
            SqlCommand command = new SqlCommand(query, Connessione);
            //Esecuzione della query richiesta
            return command.ExecuteReader();
        }

        public bool VerificaEsistenzaDato(string query)
        {

            SqlDataReader reader = LettoreComando(query);

            if (reader.Read())
            {
                const int IND = 0;
                Risultato = new RisultatoSingolo(reader.GetValue(IND), reader.GetDataTypeName(IND));
                reader.Close();
                return true;
            }
            else
            {
                reader.Close();
                return false;
            }

        }

        public RisultatoSingolo Risultato { get; private set; }


    }
}

