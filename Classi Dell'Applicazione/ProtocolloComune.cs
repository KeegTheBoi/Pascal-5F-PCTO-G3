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

        public SqlDatabase Database { get; private set; }

        private Richiesta Richiesta { get; set; }

        public object Risposta { get; set; }

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
                return new Utente(Richiesta.Utente, Richiesta.Password, 0);
            else if (Richiesta.CodiceRichiesta == 1)
                return new Operazione(Richiesta.CodiceOperazione);
                
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

        //aggiungere commento refactoring
        //Possibile podifica all'indice
        public OperazionePossibile[] ElencodiOperazioniDisponibili(int ruolo)
        {
            string queryOpers = $"SELECT * FROM OperazioniPossibili;";
            DataTable tabellaOperazioni = Database.VisualizzazioneTabella(queryOpers);

            //Lista di operazioni possibili
            List<OperazionePossibile> listOperazioni = new List<OperazionePossibile>();
            int i = 1;
            foreach (DataRow record in tabellaOperazioni.Rows)
            {
                //Selezione dei vari dati della specifica colonna e conversione
                int codOpe = int.Parse(record.ItemArray[0].ToString());
                //booleana che può anche essere null
                bool? carico;
                if (i >= 13)
                    carico = null;               
                else
                    carico = bool.Parse(record.ItemArray[1].ToString());

                string descri = (string)record.ItemArray[2];

                if (ruolo == 1 && i < 12)
                {
                    listOperazioni.Add(new OperazionePossibile(codOpe, carico, descri));
                }
                else if (ruolo == 0 && i >= 13)
                {
                    
                    listOperazioni.Add(new OperazionePossibile(codOpe, carico, descri));
                }
                i++;
            }
            _feedback += "\nInvio della lista richiesta";
            //Creazione della risposta da serielizzare
            return listOperazioni.ToArray();
            
        }

        public int OttieniRuolo(string idUt)
        {
            string queryRuolo = $"SELECT Ruolo FROM Utenti WHERE Utente = '{idUt}'";
            if (Database.VerificaEsistenzaDato(queryRuolo))
            {
                return Database.Risultato.ConvertiIntero();
            }
            else throw new Exception("ruolo inesistente");
        }


        public void InstauraConnessioneDatabase()
        {
            try
            {

                if (_validate)
                {
                    Database.IniziaConessione();
                    if (TipoRichiesta().GetType() == typeof(Utente))
                    {
                        _feedback += $"\nRichiesta di Verifica Credenziali; Codice Richiesta: {Richiesta.CodiceRichiesta}";
                        if (VerificaCredenziali((Utente)TipoRichiesta()))
                        {
                            UtenteCorrente = (Utente)TipoRichiesta();
                            UtenteCorrente.Ruolo = OttieniRuolo(UtenteCorrente.IDUtente);
                            _feedback += "\n[Server] Invio elenco di operazioni effettuabili";
                            Risposta = ElencodiOperazioniDisponibili(UtenteCorrente.Ruolo);
                        }
                    }
                    else if (TipoRichiesta().GetType() == typeof(Operazione))
                    {
                        _feedback += $"\nRichiesta di esecuzione di certe operazioni; Codice Richiesta: {Richiesta.CodiceRichiesta}";
                        Operazione op = (Operazione)TipoRichiesta();
                        if (VerificaOperazione(op))
                        {
                            string successo = "Operazione eseguita con successo";
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
            //Gestione Operazione --> Specificare il tipo di operazione

            switch(op.CodOperazione)
            {
                case 13:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        GestioneUtente utentNuovo = new GestioneUtente(Richiesta.NuovoIDUtente, Richiesta.NomeUtente, calcoloSHA256(Richiesta.NuovaPassword), Richiesta.NuovoRuolo);
                        UtenteCorrente.AggiungiUtente(Database, utentNuovo);
                        return true;
                    }
                    else
                        throw new Exception("Utenete non autorizzato");
                case 15:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        //-1 significa che non viene specificato il ruolo, da evitare di utilizzare 0 per non confondersi.
                        GestioneUtente utenteDaTogliere = new GestioneUtente(Richiesta.NuovoIDUtente, null, null, -1);
                        UtenteCorrente.RimuoviUtente(Database, utenteDaTogliere);
                        return true;
                    }
                    else
                        throw new Exception("Utenete non autorizzato");
                case 14:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        GestioneProdotto prodNuovo = new GestioneProdotto(Richiesta.NomeProdotto, 0, Richiesta.CodiceProdotto);
                        UtenteCorrente.AggiungiProdotto(Database, prodNuovo);
                        return true;
                    }
                    else
                        throw new Exception("Utente non autorizzato");
                case 16:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        GestioneProdotto prodRimuo = new GestioneProdotto(null, -1, Richiesta.CodiceProdotto);
                        UtenteCorrente.RimuoviProdotto(Database, prodRimuo);
                        return true;
                    }
                    else
                        throw new Exception("Utente non autorizzato");
                default:
                    if (UtenteCorrente.Ruolo > 0)
                    {
                        string queryOper = $"SELECT IDOperazione FROM OperazioniPossibili WHERE IDOperazione = {op.CodOperazione};";
                        if (Database.VerificaEsistenzaDato(queryOper))
                        {
                            string queryCarico = $"SELECT Carico FROM OperazioniPossibili WHERE IDOperazione = {op.CodOperazione};";
                            if (Database.VerificaEsistenzaDato(queryCarico))
                            {
                                bool carico = Database.Risultato.ConvertiBool();
                                GestioneProdotto prod = new GestioneProdotto(null, Richiesta.Quantita, Richiesta.CodiceProdotto);
                                if (carico)
                                {
                                    UtenteCorrente.CaricaProdotto(Database, prod);
                                    InserimentoOperazioneEffettuata(op, prod);
                                    return true;
                                }
                                else
                                {
                                    UtenteCorrente.ScaricaProdotto(Database, prod);
                                    InserimentoOperazioneEffettuata(op, prod);
                                    return true;
                                }
                            }
                            else
                                //Sarebbe ridondante ma serve a ottenere la booleana carico
                                throw new Exception("Carico non esistente");
                        }
                        else
                            throw new Exception("Operazione non accettata");
                    }
                    else
                        throw new Exception("Utente non autorizzato");
            }           
            
        }

        public void InserimentoOperazioneEffettuata(Operazione op, GestioneProdotto prod)
        {
            //Usare le proprieta di ope
            //Gestire il fatto se l'utente esiste
            string inserMov = $"INSERT INTO StoricoOperazioni(BarCode, CodOperazione, CodUtente, Quantità) VALUES ('{prod.IDProdotto}', {op.CodOperazione}, '{UtenteCorrente.IDUtente}', {prod.QuantitaProdotti});";
            Database.CreateUpdateDeleteRecord(inserMov);
        }

        public DataTable GetTableOperazioni()
        {
            try
            {
                Database.IniziaConessione();
                //gestire meglio con i join
                string queryMov = "SELECT IDEffettuazioniOperazioni, OperazioniPossibili.Descrizione, [Nome Prodotto], [Nome Utente], StoricoOperazioni.Quantità, CodOperazione FROM Utenti" +
                    " INNER JOIN StoricoOperazioni ON CodUtente = Utenti.Utente" +
                    " INNER JOIN Magazzino ON BarCode = Magazzino.IDProdotto " +
                    " INNER JOIN OperazioniPossibili ON CodOperazione = OperazioniPossibili.IDOperazione";
                return Database.VisualizzazioneTabella(queryMov);
            }
            catch (Exception ex)
            {
                Risposta = ex.Message;
                return null;
            }
            finally
            {
                Database.ChiudiConnessione();
            }
        }
    }

    public static class Azioni
    {
        static public void AggiungiUtente(this Utente ut, SqlDatabase database, GestioneUtente newUtente)
        {

            string selectUt = $"SELECT * FROM Utenti WHERE Utente = '{newUtente.IDUtente}'";
            if (!database.VerificaEsistenzaDato(selectUt))
            {
                string insertUtente= $"INSERT INTO Utenti(Utente, [Nome Utente], Password, Ruolo ) VALUES ('{newUtente.IDUtente}', '{newUtente.Nome}', '{newUtente.Password}', {newUtente.Ruolo});";
                database.CreateUpdateDeleteRecord(insertUtente);
            }
            else
                throw new Exception("IDUtente già esistente");
        }

        static public void RimuoviUtente(this Utente ut, SqlDatabase database, GestioneUtente uDaRimuovere)
        {
            
            string selectUt = $"SELECT * FROM Utenti WHERE Utente = '{uDaRimuovere.IDUtente}'";
            if (database.VerificaEsistenzaDato(selectUt))
            {
                string rimuovi = $"DELETE FROM Utenti WHERE Utente = '{uDaRimuovere.IDUtente}'";
                database.CreateUpdateDeleteRecord(rimuovi);
            }
            else
                throw new Exception("Utente non esistente nel database");
        }

        static public void AggiungiProdotto(this Utente ut, SqlDatabase database, GestioneProdotto newProdotto)
        {
            string selectPr = $"SELECT * FROM Magazzino WHERE IDProdotto = '{newProdotto.IDProdotto}'";
            if (!database.VerificaEsistenzaDato(selectPr))
            {
                string insertProduct = $"INSERT INTO Magazzino(IDProdotto, [Nome Prodotto], Quantità) VALUES ('{newProdotto.IDProdotto}', '{newProdotto.NomeProdotto}', {newProdotto.QuantitaProdotti});";
                database.CreateUpdateDeleteRecord(insertProduct);
            }
            else
                throw new Exception("Prodotto già esistente");
        }

        static public void RimuoviProdotto(this Utente ut, SqlDatabase database, GestioneProdotto pDaRimuovere)
        {
            string selectPr = $"SELECT * FROM Magazzino WHERE IDProdotto = '{pDaRimuovere.IDProdotto}'";
            if (database.VerificaEsistenzaDato(selectPr))
            {
                string rimuoviPr = $"DELETE FROM Magazzino WHERE IDProdotto = '{pDaRimuovere.IDProdotto}';";
                database.CreateUpdateDeleteRecord(rimuoviPr);
            }
            else
                throw new Exception("Prodotto non presente nel Database");
        }

        static public void CaricaProdotto(this Utente ut, SqlDatabase database, GestioneProdotto prod)
        {
            string selectPr = $"SELECT * FROM Magazzino WHERE IDProdotto = '{prod.IDProdotto}'";
            if (database.VerificaEsistenzaDato(selectPr))
            {
                string queryAdd = $"UPDATE Magazzino SET Quantità = Quantità + {prod.QuantitaProdotti} WHERE IDProdotto = '{prod.IDProdotto}';";
                database.CreateUpdateDeleteRecord(queryAdd);
            }
            else
                throw new Exception("Prodotto non presente nel Database");
            
        }

        static public void ScaricaProdotto(this Utente ut, SqlDatabase database, GestioneProdotto prod)
        {
            string selectPr = $"SELECT * FROM Magazzino WHERE IDProdotto = '{prod.IDProdotto}'";
            if (database.VerificaEsistenzaDato(selectPr))
            {
                string queryLeave = $"UPDATE Magazzino SET Quantità = Quantità - {prod.QuantitaProdotti} WHERE IDProdotto = '{prod.IDProdotto}';";
                database.CreateUpdateDeleteRecord(queryLeave);
            }
            else
                throw new Exception("Prodotto non presente nel Database");

        }
    }
}
