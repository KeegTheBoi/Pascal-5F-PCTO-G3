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
using System.Threading;

namespace ServerGestioneMagazzino
{
    class ProtocolloComune
    {
        public ProtocolloComune(SqlDatabase sql)
        {
            Database = sql;
        }

        public string MexJson { get; set; }

        public string _feedback;

        private bool _validate;
        public bool _successo;

        public SqlDatabase Database { get; private set; }

        private Richiesta Richiesta { get; set; }

        public object Risposta { get; set; }

        public Utente UtenteCorrente { get; private set; }

        public bool _closed;

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

        private bool ValidateJSON(string s, out string error)
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
                Risposta = new Esito(false, error);
                _feedback += String.Format("\nQualcosa è andato storto: {0} ", error + "\n[Server] Invio Messaggio d'Errore");
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
            else if (Richiesta.CodiceRichiesta == 2)
                return "exit";

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
                _feedback += "\nCifratura della password";
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

        public Movimento[] ListaStorico { get; set; }

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
                _closed = false;
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
                        _successo = false;
                        if (UtenteCorrente != null)
                        {
                            _feedback += $"\nRichiesta di esecuzione di certe operazioni; Codice Richiesta: {Richiesta.CodiceRichiesta}";
                            Operazione op = (Operazione)TipoRichiesta();
                            if (VerificaOperazione(op, out string mesRisposta))
                            {
                                
                                _successo = true;
                                _feedback += "\n" + mesRisposta + "\n[Server] Invio Esito dell'operazione";

                                if (op.CodOperazione != 17)
                                    Risposta = new Esito(true, mesRisposta);
                                else
                                    Risposta = ListaStorico;
                            }
                        }
                        else
                            throw new Exception("Utente non connesso");

                    }
                    else if ((string)TipoRichiesta() == "exit")
                    {
                        Risposta = new Esito(true, "Richiesta di chiusura ricevuta");
                        _feedback += "\nClient abbandona la comunicazione \n[Server] Invio riscontro di chiusura";
                        UtenteCorrente = null;
                        _closed = true;
                    }
                    else
                    {
                        throw new Exception("Codice richiesta non riconosciuto");
                    }
                }
                else return;

            }
            catch (Exception ex)
            {
                Risposta = new Esito(false, ex.Message);
                _feedback += String.Format("\nQualcosa è andato storto: {0} ", ex.Message + "\n[Server] Invio Messaggio d'Errore");
            }
            finally
            {
                Database.ChiudiConnessione();
            }
        }



        public bool VerificaOperazione(Operazione op, out string response)
        {
            //Gestione Operazione --> Specificare il tipo di operazione

            switch (op.CodOperazione)
            {
                case 13:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        _feedback += "\nUtente Autorizzato, Grado di accesso: 0";
                        GestioneUtente utentNuovo = new GestioneUtente(Richiesta.NuovoIDUtente, Richiesta.NomeUtente, calcoloSHA256(Richiesta.NuovaPassword), Richiesta.NuovoRuolo);
                        UtenteCorrente.AggiungiUtente(Database, utentNuovo);
                        _feedback += "\nAggiunta di un Utente avvenuto con successo";
                        response = $"Utente {utentNuovo.IDUtente} aggiunto";
                        return true;
                    }
                    else
                        throw new Exception("Utente non autorizzato, Autorizzati solo utenti con grado di ruolo 0");
                case 15:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        _feedback += "\nUtente Autorizzato, Grado di accesso: 0";
                        //-1 significa che non viene specificato il ruolo, da evitare di utilizzare 0 per non confondersi.
                        GestioneUtente utenteDaTogliere = new GestioneUtente(Richiesta.NuovoIDUtente, null, null, -1);
                        UtenteCorrente.RimuoviUtente(Database, utenteDaTogliere);
                        _feedback += "\nEliminazione di un Utente avvenuto con successo";
                        response = $"Utente {utenteDaTogliere.IDUtente} rimosso";
                        return true;
                    }
                    else
                        throw new Exception("Utente non autorizzato, Autorizzati solo utenti con grado di ruolo 0");
                case 14:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        _feedback += "\nUtente Autorizzato, Grado di accesso: 0";
                        GestioneProdotto prodNuovo = new GestioneProdotto(Richiesta.NomeProdotto, 0, Richiesta.CodiceProdotto);
                        UtenteCorrente.AggiungiProdotto(Database, prodNuovo);
                        _feedback += "\nAggiunta di un prodotto avvenuto con successo";
                        response = $"Nuovo prodotto {prodNuovo.IDProdotto} aggiunto";
                        return true;
                    }
                    else
                        throw new Exception("Utente non autorizzato, Autorizzati solo utenti con grado di ruolo 0");
                case 16:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        _feedback += "\nUtente Autorizzato, Grado di accesso: 0";
                        GestioneProdotto prodRimuo = new GestioneProdotto(null, -1, Richiesta.CodiceProdotto);
                        UtenteCorrente.RimuoviProdotto(Database, prodRimuo);
                        _feedback += "\nEliminazione di un prodotto avvenuto con successo";
                        response = $"Prodotto {prodRimuo.IDProdotto} rimosso";
                        return true;
                    }
                    else
                        throw new Exception("Utente non autorizzato, Autorizzati solo utenti con grado di ruolo 0");
                case 17:
                    if (UtenteCorrente.Ruolo == 0)
                    {
                        _feedback += "\nUtente Autorizzato, Grado di accesso: 0";
                        List<Movimento> listMovimento = UtenteCorrente.VisualizzaStorico(Database, Richiesta.SceltaOperazione);
                        ListaStorico = listMovimento.ToArray();
                        if (ListaStorico.Length == 0)
                            throw new Exception("Non è stato trovato alcun movimento");
                        _feedback += "\nMovimenti Esistenti";
                        response = "";
                        return true;
                    }
                    else
                        throw new Exception("Utente non autorizzato, Autorizzati solo utenti con grado di ruolo 0");
                default:
                    if (UtenteCorrente.Ruolo > 0)
                    {
                        _feedback += "\n Utente Autorizzato, Grado di accesso: 1";
                        string queryOper = $"SELECT IDOperazione FROM OperazioniPossibili WHERE IDOperazione = {op.CodOperazione};";
                        if (Database.VerificaEsistenzaDato(queryOper))
                        {
                            string queryCarico = $"SELECT Carico FROM OperazioniPossibili WHERE IDOperazione = {op.CodOperazione};";
                            if (Database.VerificaEsistenzaDato(queryCarico))
                            {

                                bool carico = Database.Risultato.ConvertiBool();
                                GestioneProdotto prod = new GestioneProdotto(null, Richiesta.Quantita, Richiesta.CodiceProdotto);
                                string queryQUantita = $"SELECT Quantità FROM Magazzino WHERE IDProdotto = '{prod.IDProdotto}'";
                                if (carico)
                                {
                                    UtenteCorrente.CaricaProdotto(Database, prod);
                                    InserimentoOperazioneEffettuata(op, prod);

                                    Database.VerificaEsistenzaDato(queryQUantita);
                                    _feedback += "\nCarico del Prodotto avvenuto con successo;\nQuantità rimasta: " + Database.Risultato.ConvertiIntero();
                                    response = $"Carico del Prodotto {prod.IDProdotto} di quantita {prod.QuantitaProdotti} avvenuto con successo. Quantita rimasta: " + Database.Risultato.ConvertiIntero();
                                    return true;
                                }
                                else
                                {
                                    UtenteCorrente.ScaricaProdotto(Database, prod);
                                    InserimentoOperazioneEffettuata(op, prod);

                                    Database.VerificaEsistenzaDato(queryQUantita);
                                    _feedback += "\nCarico del Prodotto avvenuto con successo;\nQuantità rimasta: " + Database.Risultato.ConvertiIntero();
                                    response = $"Scarico del Prodotto {prod.IDProdotto} di quantita {prod.QuantitaProdotti} avvenuto con successo. Quantita rimasta: " + Database.Risultato.ConvertiIntero();
                                    return true;
                                }
                            }
                            else
                                //Sarebbe ridondante ma serve a ottenere la booleana carico
                                throw new Exception("Carico non esistente");
                        }
                        else
                            throw new Exception("Operazione non accettata, Operazioni accettate [1-17]");
                    }
                    else
                        throw new Exception("Utente non autorizzato, Autorizzati solo utenti con grado di ruolo 1");
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
                string queryMov = "SELECT IDEffettuazioniOperazioni, OperazioniPossibili.Descrizione, [Nome Prodotto], [Nome Utente], StoricoOperazioni.Quantità, CodOperazione FROM Utenti INNER JOIN" +
                    "  StoricoOperazioni ON CodUtente = Utenti.Utente INNER JOIN" +
                    "  Magazzino ON BarCode = Magazzino.IDProdotto INNER JOIN" +
                    "  OperazioniPossibili ON CodOperazione = OperazioniPossibili.IDOperazione";

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

    
}
