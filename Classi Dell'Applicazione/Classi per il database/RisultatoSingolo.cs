using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace GestioneDatabases
{
    public class RisultatoSingolo
    {
        public RisultatoSingolo(object value, string tipo)
        {
            Data = value;
            _type = tipo;
        }

        object Data { get; set; }

        string _type;

        public bool ConvertiBool()
        {
            if (_type == "bit")
                return (bool)Data;
            else throw new Exception("Impossibile convertire in un bool");
        }

        public int ConvertiIntero()
        {
            if (_type == "Int32")
                return Int32.Parse(Data.ToString());
            else throw new Exception("Impossibile convertire in un intero");
        }

        public string ConvertStringa()
        {
            if (_type == "char")
                return Data.ToString();
            else throw new Exception("Impossibile convertire in una stringa");
        }
    }
}
