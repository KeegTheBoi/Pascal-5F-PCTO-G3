using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtocolloGenerale;
using System.Data;
using GestioneDatabases;

namespace ServerGestioneMagazzino
{
    public static class Azioni
    {

        static public void AggiungiUtente(this Utente ut, SqlDatabase database, GestioneUtente newUtente)
        {

            string selectUt = $"SELECT * FROM Utenti WHERE Utente = '{newUtente.IDUtente}'";
            if (!database.VerificaEsistenzaDato(selectUt))
            {
                string insertUtente = $"INSERT INTO Utenti(Utente, [Nome Utente], Password, Ruolo ) VALUES ('{newUtente.IDUtente}', '{newUtente.Nome}', '{newUtente.Password}', {newUtente.Ruolo});";
                database.CreateUpdateDeleteRecord(insertUtente);
            }
            else
                throw new Exception("IDUtente gia esistente");
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
                throw new Exception("Prodotto gia esistente");
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

        public static List<Movimento> VisualizzaStorico(this Utente ut, SqlDatabase database, int scelta)
        {
            if (scelta < 4 && scelta > 0)
            {
                string query = "";
                if (scelta == 1)
                {
                    query = $"SELECT IDEffettuazioniOperazioni,BarCode,CodOperazione,CodUtente,Quantità,Carico FROM StoricoOperazioni AS SO INNER JOIN OperazioniPossibili AS OP ON SO.CodOperazione = OP.IDOperazione WHERE CodOperazione = 1 OR CodOperazione = 2 OR CodOperazione = 4 OR CodOperazione <= 5 OR CodOperazione = 11;";
                }
                else if (scelta == 2)
                {
                    query = $"SELECT IDEffettuazioniOperazioni,BarCode,CodOperazione,CodUtente,Quantità,Carico FROM StoricoOperazioni AS SO INNER JOIN OperazioniPossibili AS OP ON SO.CodOperazione = OP.IDOperazione WHERE (CodOperazione >= 6 AND CodOperazione <= 10) OR CodOperazione = 12;";
                }
                else if (scelta == 3)
                {
                    query = $"SELECT IDEffettuazioniOperazioni,BarCode,CodOperazione,CodUtente,Quantità,Carico FROM StoricoOperazioni AS SO INNER JOIN OperazioniPossibili AS OP ON SO.CodOperazione = OP.IDOperazione WHERE SO.CodOperazione=3;";
                }
                DataTable tabella = database.VisualizzazioneTabella(query);
                List<Movimento> listaMovimento = new List<Movimento>();
                for (int i = 0; i < tabella.Rows.Count; i++)
                {
                    DataRow dt = tabella.Rows[i];
                    listaMovimento.Add(new Movimento(dt.ItemArray[3].ToString(), dt.ItemArray[1].ToString(), bool.Parse(dt.ItemArray[5].ToString()), int.Parse(dt.ItemArray[4].ToString()), int.Parse(dt.ItemArray[0].ToString()), int.Parse(dt.ItemArray[2].ToString())));
                }
                return listaMovimento;
            }
            else
                throw new Exception("Codice Scelta non valido");

        }

    }
}
