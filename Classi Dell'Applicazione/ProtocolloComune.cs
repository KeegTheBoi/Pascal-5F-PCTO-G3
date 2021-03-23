using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtocolloGenerale;
using GestioneDatabases;
using System.Security.Cryptography;
using System.Text;


namespace ServerGestioneMagazzino
{
    class ProtocolloComune
    {
        public ProtocolloComune(SqlDatabase sql)
        {
            Database = sql;
        }

        public string MexJson { get;  set; }

        public string _feedback;

        private bool _validate;

        private SqlDatabase Database { get; set; }

        private Richiesta Richiesta { get; set; }

        public Risposta Risposta { get; set; }

        public Utente UtenteCorrente { get; private set; }

        
        public string calcoloSHA256(string passwd)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(passwd));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void ConversioneMessaggio()
        {
            //Verifica se si può convertire il file in Json
            //Altrimenti viene segnalato al client l'accaduto
            if (ValidateJSON(MexJson, out string errore))
            {
                Richiesta = JsonConvert.DeserializeObject<Richiesta>(MexJson);
                _feedback = "";
                _feedback += "\nProtocollo Usato: Protocollo Generale; Formato: JSON";
                _validate = true;
            }
            else _validate = false;

        }

        private static bool ValidateJSON(string s, out string error)
        {
            error = "";
            try
            {
                var obj = JsonConvert.DeserializeObject<Richiesta>(s);
                return true;
            }
            catch (JsonReaderException ex)
            {
                error = "Il sistema ha riscontrato una errore\nSpecifiche: " + ex.Message;
                return false;
            }
        }

        private object TipoRichiesta()
        {
            //Creazione degli Oggetti necessari alla convalida

            if (Richiesta.CodiceRichiesta == 0)
                return new Utente(Richiesta.Utente, Richiesta.Password);
            else if (Richiesta.CodiceRichiesta == 1)
                return new Operazione(Richiesta.CodiceProdotto, Richiesta.NomeProdotto, Richiesta.Quantita, Richiesta.CodiceOperazione);

            return -1;
        }

        private bool VerificaCredenziali(Utente u)
        {
            bool verify = false;
            _feedback += $"\n[Client] Utente: {u.IDUtente}";
            string queryUtente = $"SELECT Utente FROM Utenti WHERE Utente = '{u.IDUtente}';";
            if (Database.VerificaEsistenzaDato(queryUtente))
            {
                _feedback += "\nUtente esistente\nVerificazione Password";
                u.Password = calcoloSHA256(u.Password);
                string queryPass = $"SELECT [Nome Utente] FROM Utenti WHERE Utente = '{u.IDUtente}' AND Password = '{u.Password}';";
                if (Database.VerificaEsistenzaDato(queryPass))
                {
                    _feedback += $"\nAutenticazione effettuata.\nUtente: {u.IDUtente} | Nome Utente: {Database.Risultato.ConvertStringa()}";
                    verify = true;
                    return verify;
                }
                else
                {
                    _feedback += "\nInserimento Incorretto";
                    throw new Exception("Utente o Password Incorretti");
                }
            }
            else
            {
                _feedback += "\nUtente non trovato";
                throw new Exception("Utente inesistente");
            }

        }

        public Risposta ElencodiOperazioniDisponibili()
        {
            string queryOpers = $"SELECT * FROM OperazioniPossibili;";
            DataTable tabellaOperazioni = Database.VisualizzazioneTabella(queryOpers);

            //Lista di operazioni possibili
            List<OperazionePossibile> listOperazioni = new List<OperazionePossibile>();
            foreach (DataRow record in tabellaOperazioni.Rows)
            {
                //Selezione dei vari dati della specifica colonna e conversione
                int codOpe = int.Parse(record.ItemArray[0].ToString());
                bool carico = bool.Parse(record.ItemArray[1].ToString());
                string descri = (string)record.ItemArray[2];
                listOperazioni.Add(new OperazionePossibile(codOpe, carico, descri));
            }

            //Creazione della risposta da serielizzare
            return new ElencoOperazioni("", listOperazioni.ToArray());
            //da immettere un messaggio di feedback positivo
        }

        public void InstauraConnessioneDatabase()
        {
            try
            {
                if (_validate)
                {
                    if (TipoRichiesta().GetType() == typeof(Utente))
                    {
                        _feedback += $"\nRichiesta di Verifica Credenziali; Codice Richiesta: {Richiesta.CodiceRichiesta}";
                        if (VerificaCredenziali((Utente)TipoRichiesta()))
                        {
                            UtenteCorrente = (Utente)TipoRichiesta();
                            _feedback += "\n[Server] Invio elenco di operazioni effettuabili";
                            Risposta = ElencodiOperazioniDisponibili();
                        }
                    }
                    else if (TipoRichiesta().GetType() == typeof(Operazione))
                    {
                        _feedback += $"\nRichiesta di Inserimento; Codice Richiesta: {Richiesta.CodiceRichiesta}";
                        Operazione op = (Operazione)TipoRichiesta();
                        if (VerificaOperazione(op))
                        {
                            InserimentoOperazione(op);
                            string successo = "Inserimento avvenuto con successo";
                            _feedback += "\n" + successo + "\n[Server] Invio Esito dell'operazione";
                            Risposta = new Esito(true, successo);
                        }
                    }
                    else
                    {
                        throw new Exception("Codice richiesta non riconosciuto");
                    }
                }
                else return;
                
            }
            catch(Exception ex)
            {
                Risposta = new Esito(false, ex.Message);
                _feedback += String.Format("\nQualcosa è andato storto: {0} ", ex.Message + "\n[Server] Invio Messaggio d'Errore"); 
            }
            finally
            {
                Database.ChiudiConnessione();
            }
        }

        public bool VerificaOperazione(Operazione op)
        {
            string queryOper = $"SELECT IDOperazione FROM OperazioniPossibili WHERE IDOperazione = {op.CodOperazione};";
            _feedback += $"\n[Client] Codice Operazione: {op.CodOperazione} | Prodotto: {op.NomeProdotto} | Quantita: {op.QuantitaProdotti}";
            if (Database.VerificaEsistenzaDato(queryOper))
            {
                _feedback += $"\nCodice Operazione Esistente";
                string queryProd = $"SELECT IDProdotto FROM Magazzino WHERE IDProdotto = '{op.IDProdotto}';";
                if(!Database.VerificaEsistenzaDato(queryProd))
                {
                    _feedback += $"\nProdoto Inesistente\nInizializzione del processo di inserimento del Prodotto";
                    string inserProd = $"INSERT INTO Magazzino(IDProdotto, Descrizione, Quantità) VALUES ('{op.IDProdotto}', '{op.NomeProdotto}', {op.QuantitaProdotti});";
                    Database.InserimentoDati(inserProd);
                    _feedback += $"\nInserimento avvenuto con successo";
                }
                //Serve solo ad ottenere il tipo di carico/scarico
                string queryCarico = $"SELECT Carico FROM OperazioniPossibili WHERE IDOperazione = {op.CodOperazione};";
                if(Database.VerificaEsistenzaDato(queryCarico))
                {
                    bool carico = (bool)Database.Risultato.ConvertiBool();
                    if(carico)
                    {
                        _feedback += $"\nOperazione di Carico: Aggiunta di {op.QuantitaProdotti} prodotti";
                        string queryAdd = $"UPDATE Magazzino SET Quantità = Quantità + {op.QuantitaProdotti} WHERE IDProdotto = '{op.IDProdotto}';";
                        Database.AggoirnaDato(queryAdd);
                        _feedback += $"\nCarico avvenuto con successo";
                        return true;
                    }
                    else
                    {
                        _feedback += $"\nOperazione di Scarico: Sottrazione di {op.QuantitaProdotti} prodotti";
                        string queryLeave = $"UPDATE Magazzino SET Quantità = Quantità - {op.QuantitaProdotti} WHERE IDProdotto = '{op.IDProdotto}';";
                        Database.AggoirnaDato(queryLeave);
                        _feedback += $"\nScarico avvenuto con successo";
                        return true;
                    }

                }
                else
                {
                    throw new Exception("Carico o scarico Insesitente");
                }
            }
            else
            {
                throw new Exception("Identificativo operazione inesistente");
            }
        }

        public void InserimentoOperazione(Operazione ope)
        {
            //Usare le proprieta di ope
            //Gestire il fatto se l'utente esiste
            string inserMov = $"INSERT INTO StoricoOperazioni(BarCode, CodOperazione, CodUtente, Quantità) VALUES ('{ope.IDProdotto}', {ope.CodOperazione}, '{UtenteCorrente.IDUtente}', {ope.QuantitaProdotti});";
            Database.InserimentoDati(inserMov);
        }

        public DataTable GetTableOperazioni()
        {
            //gestire meglio con i join
            string queryMov = "SELECT * FROM StoricoOperazioni";
            return Database.VisualizzazioneTabella(queryMov);
        }
    }
}
