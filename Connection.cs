using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace AdHocRevolutionGlamour
{
    public class Connection
    {
        public string schema;
        public string schema_info;
        public string azienda;
        public SqlConnection connection;

        Dictionary<string, string> colori;
        Dictionary<string, string> numerate;
        List<string> taglie;

        Dictionary<string, string> tipiBarcode = new Dictionary<string, string>() {
            { "", "" },
            { "R", "Interno" },
            { "F", "Fornitore" },
        };
        Dictionary<string, string> codificheBarcode = new Dictionary<string, string>() {
            { "", "" },
            { "2", "EAN 13" },
            { "3", "ALFA 39" },
            { "4", "UPC A" }
        };

        public Connection(string connetionString, string schema, string schema_info, string azienda)
        {
            this.schema = schema;
            this.schema_info = schema_info;
            this.azienda = azienda;
            connection = new SqlConnection(connetionString);
        }

        public Connection(SqlConnection connection, string schema, string schema_info, string azienda)
        {
            this.schema = schema;
            this.schema_info = schema_info;
            this.azienda = azienda;
            this.connection = connection;
        }

        public void Open()
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public void Close()
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }

        public Dictionary<string, string> GetCodificheBarcode()
        {
            return codificheBarcode;
        }

        public Dictionary<string, string> GetTipiBarcode()
        {
            return tipiBarcode;
        }

        public Dictionary<string, string> GetNumerate()
        {
            numerate = new Dictionary<string, string>() { { "", "" } };
            string query = "SELECT NUCODNUM, NUDESNUM FROM [" + schema + "].[dbo].[" + azienda + "TCNUMERA]";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    numerate.Add(reader[0].ToString().Trim(), reader[1].ToString().Trim());
                }
                reader.Close();
            }
            command.Dispose();
            return numerate;
        }

        public Dictionary<string, string> GetMarche()
        {
            numerate = new Dictionary<string, string>() { { "", "" } };
            string query = "SELECT MACODICE, MADESCRI FROM [" + schema + "].[dbo].[" + azienda + "MARCHI]";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    numerate.Add(reader[0].ToString().Trim(), reader[1].ToString().Trim());
                }
                reader.Close();
            }
            command.Dispose();
            return numerate;
        }

        public List<string> GetTaglie(string Numerata)
        {
            taglie = new List<string>();
            string query = "SELECT * FROM [" + schema + "].[dbo].[" + azienda + "TCNUMERA] WHERE NUCODNUM = '" + Numerata + "'";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    for (int i = 0; i < 30; i++)
                    {
                        string indiceTaglia = (i + 1).ToString().PadLeft(2, '0');
                        string Taglia = reader["NUNUME" + indiceTaglia].ToString().Trim();
                        if (!string.IsNullOrEmpty(Taglia))
                            taglie.Add(Taglia);
                    }
                }
                reader.Close();
            }
            command.Dispose();
            return taglie;
        }

        public Dictionary<string, string> GetColori()
        {
            colori = new Dictionary<string, string>() { { "", "" } };
            string query = "SELECT COCODCOL, CODESCOL FROM [" + schema + "].[dbo].[" + azienda + "TCCOLORI]";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    colori.Add(reader[0].ToString().Trim(), reader[1].ToString().Trim());
                }
                reader.Close();
            }
            command.Dispose();
            return colori;
        }

        public Dictionary<string, string> GetFornitori()
        {
            colori = new Dictionary<string, string>() { { "", "" } };
            string query = "SELECT ANCODICE, ANDESCRI FROM [" + schema + "].[dbo].[" + azienda + "CONTI] WHERE ANTIPCON = 'F'";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    colori.Add(reader[0].ToString().Trim(), reader[1].ToString().Trim());
                }
                reader.Close();
            }
            command.Dispose();
            return colori;
        }

        public DataTable GetCodici(
            string codiceArticolo,
            string codiceColore = null,
            string codiceTaglia = null,
            string codiceFornitore = null,
            string codiceMarca = null,
            string tipoBarcode = null,
            string codificaBarcode = null
        )
        {
            DataTable codici = new DataTable
            {
                TableName = "Codici"
            };
            codici.Columns.Add("Barcode");
            codici.Columns.Add("CodiceArticolo");
            codici.Columns.Add("DescrizioneArticolo");
            codici.Columns.Add("DescrizioneSupplementareArticolo");
            codici.Columns.Add("CodiceColore");
            codici.Columns.Add("DescrizioneColore");
            codici.Columns.Add("CodiceNumerata");
            codici.Columns.Add("DescrizioneNumerata");
            codici.Columns.Add("CodiceTaglia");
            codici.Columns.Add("Prezzo");
            codici.Columns.Add("TipoBarcode");
            codici.Columns.Add("CodificaBarcode");
            codici.Columns.Add("Quantita");
            codici.Columns.Add("CodiceMadeIn");
            codici.Columns.Add("CodiceMarca");
            codici.Columns.Add("CodiceStagione");
            codici.Columns.Add("CodiceGenere");
            codici.Columns.Add("CodiceGruppoMerceologico");
            codici.Columns.Add("CodiceCategoriaOmogenea");
            codici.Columns.Add("CodiceFornitore");
            codici.Columns.Add("NomenclaturaCombinata");
            codici.Columns.Add("FuoriProduzione");
            codici.Columns.Add("CodiceFamiglia");

            string query = "SELECT " +
                "" + azienda + "KEY_ARTI.CACODICE AS Barcode, " +
                "" + azienda + "ART_ICOL.ARCODART AS CodiceArticolo, " +
                "" + azienda + "ART_ICOL.ARDESART AS DescrizioneArticolo, " +
                "" + azienda + "ART_ICOL.ARDESSUP AS DescrizioneSupplementareArticolo, " +
                "" + azienda + "KEY_ARTI.TCCOLORE AS CodiceColore, " +
                "" + azienda + "TCCOLORI.CODESCOL AS DescrizioneColore, " +
                "" + azienda + "TCNUMERA.NUCODNUM AS CodiceNumerata, " +
                "" + azienda + "TCNUMERA.NUDESNUM AS DescrizioneNumerata, " +
                "" + azienda + "KEY_ARTI.TCTAGLIA AS CodiceTaglia, " +
                "" + azienda + "ART_ICOL.TCMADEIN AS CodiceMadeIn, " +
                "" + azienda + "ART_ICOL.ARCODMAR AS CodiceMarca, " +
                "" + azienda + "ART_ICOL.ARSTAGIO AS CodiceStagione, " +
                "" + azienda + "ART_ICOL.TCGENERE AS CodiceGenere, " +
                "" + azienda + "ART_ICOL.ARGRUMER AS CodiceGruppoMerceologico, " +
                "" + azienda + "ART_ICOL.ARCATOMO AS CodiceCategoriaOmogenea, " +
                "" + azienda + "TCSCHMAS.STCODFOR AS CodiceFornitore, " +
                "" + azienda + "TCSCHMAS.STNOMENC AS NomenclaturaCombinata, " +
                "" + azienda + "TCSCHMAS.STCODFAM AS CodiceFamiglia, " +                
                "" + azienda + "KEY_ARTI.CATIPBAR AS CodificaBarcode, " +
                "" + azienda + "KEY_ARTI.CATIPCON AS TipoBarcode " +
                "FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "ART_ICOL] ON " + azienda + "KEY_ARTI.CACODART = " + azienda + "ART_ICOL.ARCODART " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "TCCOLORI] ON " + azienda + "TCCOLORI.COCODCOL = " + azienda + "KEY_ARTI.TCCOLORE " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "TCSCHMAS] ON " + azienda + "TCSCHMAS.STCODART = " + azienda + "KEY_ARTI.CACODART " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "TCNUMERA] ON " + azienda + "TCNUMERA.NUCODNUM = " + azienda + "TCSCHMAS.STCODNUM " +
                "WHERE CACODART LIKE '" + codiceArticolo + "'";

            if (!string.IsNullOrEmpty(codiceColore))
            {
                query += " AND TCCOLORE = '" + codiceColore + "'";
            }
            if (!string.IsNullOrEmpty(codiceTaglia))
            {
                query += " AND TCTAGLIA = '" + codiceTaglia + "'";
            }
            if (!string.IsNullOrEmpty(codiceFornitore))
            {
                query += " AND STCODFOR = '" + codiceFornitore + "'";
            }
            if (!string.IsNullOrEmpty(codiceMarca))
            {
                query += " AND ARCODMAR = '" + codiceMarca + "'";
            }
            if (!string.IsNullOrEmpty(tipoBarcode))
            {
                query += " AND CATIPCON = '" + tipoBarcode + "'";
            }
            if (!string.IsNullOrEmpty(codificaBarcode))
            {
                query += " AND CATIPBAR = '" + codificaBarcode + "'";
            }

            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string CodiceArticolo = reader["CodiceArticolo"].ToString().Trim();
                    string CodiceNumerata = reader["CodiceNumerata"].ToString().Trim();
                    string CodiceTaglia = reader["CodiceTaglia"].ToString().Trim();
                    string CodiceColore = reader["CodiceColore"].ToString().Trim();
                    if (CodiceNumerata == "TU" && CodiceTaglia == "TU")
                        continue;
                    ArticoloInfo articoloInfo = GetArticoloInfo(CodiceArticolo, CodiceColore, CodiceTaglia);
                    List<string> Taglie = GetTaglie(CodiceNumerata);
                    string IndiceTaglia = (Taglie.IndexOf(CodiceTaglia) + 1).ToString().PadLeft(2, '0');
                    var row = codici.NewRow();
                    row["Barcode"] = reader["Barcode"].ToString().Trim();
                    row["CodiceArticolo"] = CodiceArticolo;
                    row["DescrizioneArticolo"] = reader["DescrizioneArticolo"].ToString().Trim();
                    row["DescrizioneSupplementareArticolo"] = reader["DescrizioneSupplementareArticolo"].ToString().Trim();
                    row["CodiceColore"] = CodiceColore;
                    row["DescrizioneColore"] = reader["DescrizioneColore"].ToString().Trim();
                    row["CodiceNumerata"] = CodiceNumerata;
                    row["DescrizioneNumerata"] = reader["DescrizioneNumerata"].ToString().Trim();
                    row["CodiceTaglia"] = CodiceTaglia;
                    row["Prezzo"] = GetPrezzo(CodiceArticolo, "VENP");
                    row["Quantita"] = GetQuantita(CodiceArticolo, CodiceColore, IndiceTaglia).ToString();
                    row["CodificaBarcode"] = reader["CodificaBarcode"].ToString().Trim();
                    row["TipoBarcode"] = reader["TipoBarcode"].ToString().Trim();
                    row["CodiceMadeIn"] = reader["CodiceMadeIn"].ToString().Trim();
                    row["CodiceMarca"] = reader["CodiceMarca"].ToString().Trim();
                    row["CodiceStagione"] = reader["CodiceStagione"].ToString().Trim();
                    row["CodiceGenere"] = reader["CodiceGenere"].ToString().Trim();
                    row["CodiceGruppoMerceologico"] = reader["CodiceGruppoMerceologico"].ToString().Trim();
                    row["CodiceCategoriaOmogenea"] = reader["CodiceCategoriaOmogenea"].ToString().Trim();
                    row["CodiceFornitore"] = reader["CodiceFornitore"].ToString().Trim();
                    row["NomenclaturaCombinata"] = reader["CodiceFornitore"].ToString().Trim();
                    row["FuoriProduzione"] = articoloInfo.FuoriProduzione ? "Si" : "No";
                    row["CodiceFamiglia"] = reader["CodiceFamiglia"].ToString().Trim();
                    codici.Rows.Add(row);
                }
                reader.Close();
            }
            command.Dispose();
            return codici;
        }

        public ArticoloInfo GetArticoloInfo (string CodiceArticolo, string CodiceColore, string CodiceTaglia)
        {
            ArticoloInfo articolo = new ArticoloInfo();
            var query = "SELECT * FROM [" + schema_info + "].[dbo].[ARTICOLI] WHERE " +
                "CodiceArticolo = @CodiceArticolo AND " +
                "CodiceColore = @CodiceColore AND " +
                "CodiceTaglia = @CodiceTaglia";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("CodiceArticolo", CodiceArticolo);
            command.Parameters.AddWithValue("CodiceColore", CodiceColore);
            command.Parameters.AddWithValue("CodiceTaglia", CodiceTaglia);
            SqlDataReader reader = command.ExecuteReader();
            articolo.CodiceArticolo = CodiceArticolo;
            articolo.CodiceColore = CodiceColore;
            articolo.CodiceTaglia = CodiceTaglia;
            articolo.FuoriProduzione = false;
            articolo.Numerata = "";
            if (reader.Read())
            {
                articolo.Numerata = reader["Numerata"].ToString().Trim();
                articolo.FuoriProduzione = (bool)reader["FuoriProduzione"];
            }
            return articolo;
        }

        public string GetPrezzo(string CodiceArticolo, string Listino)
        {
            string prezzo = string.Empty;
            var query = "SELECT * FROM [" + schema + "].[dbo].[" + azienda + "LIS_SCAG] " +
                "INNER JOIN " + azienda + "LIS_TINI ON " + azienda + "LIS_TINI.LICODART = " + azienda + "LIS_SCAG.LICODART AND " + azienda + "LIS_TINI.CPROWNUM = " + azienda + "LIS_SCAG.LIROWNUM " +
                "WHERE " + azienda + "LIS_TINI.LICODART = '" + CodiceArticolo + "' AND " + azienda + "LIS_TINI.LICODLIS = '" + Listino + "' " +
                "ORDER BY " + azienda + "LIS_TINI.LIDATATT DESC";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                for (int i = 0; i < 30; i++)
                {
                    string indiceTaglia = (i + 1).ToString().PadLeft(2, '0');
                    prezzo = reader["LIPREZ" + indiceTaglia].ToString().Trim();
                    prezzo = prezzo.Substring(0, prezzo.Length - 3);
                    if (Convert.ToDouble(prezzo) > 0)
                    {
                        break;
                    }
                }
            }
            reader.Close();
            command.Dispose();
            return prezzo;
        }

        public int GetQuantita(string CodiceArticolo, string CodiceColore, string IndiceTaglia)
        {
            int Quantita = 0;
            var query = "SELECT * FROM [" + schema + "].[dbo].[" + azienda + "TCSALGLA] " +
                "WHERE TCCODART = '" + CodiceArticolo + "' AND TCCODCOL = '" + CodiceColore + "'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                string strQuantita = reader["TCQGIA" + IndiceTaglia].ToString().Trim().Split(',').First();
                if (!string.IsNullOrEmpty(strQuantita))
                {
                    try
                    {
                        Quantita = Convert.ToInt32(strQuantita);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            reader.Close();
            command.Dispose();
            return Quantita;
        }

        public string GetBarcode(string CodiceArticolo, string CodiceColore, string Taglia)
        {
            SqlCommand command;
            SqlDataReader reader;
            string Barcode = string.Empty;
            var query = "SELECT CACODICE FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] WHERE " +
                "CACODART = '" + CodiceArticolo + "' AND " +
                "TCCOLORE = '" + CodiceColore + "' AND " +
                "TCTAGLIA = '" + Taglia + "' AND " +
                "CATIPBAR = 'F' AND CATIPCON = '2'";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Barcode = reader[0].ToString();
            }
            reader.Close();
            command.Dispose();
            if (!string.IsNullOrEmpty(Barcode))
                return Barcode;

            query = "SELECT CACODICE FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] WHERE " +
                "CACODART = '" + CodiceArticolo + "' AND " +
                "TCCOLORE = '" + CodiceColore + "' AND " +
                "TCTAGLIA = '" + Taglia + "' AND " +
                "CATIPBAR = 'R' AND CATIPCON = '2'";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Barcode = reader[0].ToString();
            }
            reader.Close();
            command.Dispose();
            if (!string.IsNullOrEmpty(Barcode))
                return Barcode;

            query = "SELECT CACODICE FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] WHERE " +
                "CACODART = '" + CodiceArticolo + "' AND " +
                "TCCOLORE = '" + CodiceColore + "' AND " +
                "TCTAGLIA = '" + Taglia + "' AND " +
                "CATIPBAR = 'R' AND CATIPCON = '4'";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Barcode = reader[0].ToString();
            }
            reader.Close();
            command.Dispose();
            if (!string.IsNullOrEmpty(Barcode))
                return Barcode;


            query = "SELECT CACODICE FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] WHERE " +
                "CACODART = '" + CodiceArticolo + "' AND " +
                "TCCOLORE = '" + CodiceColore + "' AND " +
                "TCTAGLIA = '" + Taglia + "' AND " +
                "CATIPBAR = 'F' AND CATIPCON = '4'";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Barcode = reader[0].ToString();
            }
            reader.Close();
            command.Dispose();
            if (!string.IsNullOrEmpty(Barcode))
                return Barcode;

            query = "SELECT CACODICE FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] WHERE " +
                "CACODART = '" + CodiceArticolo + "' AND " +
                "TCCOLORE = '" + CodiceColore + "' AND " +
                "TCTAGLIA = '" + Taglia + "' AND " +
                "CATIPBAR = 'R' AND CATIPCON = '3'";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Barcode = reader[0].ToString();
            }
            reader.Close();
            command.Dispose();
            if (!string.IsNullOrEmpty(Barcode))
                return Barcode;

            query = "SELECT CACODICE FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] WHERE " +
                "CACODART = '" + CodiceArticolo + "' AND " +
                "TCCOLORE = '" + CodiceColore + "' AND " +
                "TCTAGLIA = '" + Taglia + "' AND " +
                "CATIPBAR = 'F' AND CATIPCON = '3'";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Barcode = reader[0].ToString();
            }
            reader.Close();
            command.Dispose();
            if (!string.IsNullOrEmpty(Barcode))
                return Barcode;

            query = "SELECT CACODICE FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] WHERE " +
                            "CACODART = '" + CodiceArticolo + "' AND " +
                            "TCCOLORE = '" + CodiceColore + "' AND " +
                            "TCTAGLIA = '" + Taglia + "'";
            command = new SqlCommand(query, connection);
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Barcode = reader[0].ToString();
            }
            reader.Close();
            command.Dispose();
            if (!string.IsNullOrEmpty(Barcode))
                return Barcode;

            return Barcode;
        }

        public string GetColore(string CodiceColore)
        {
            string Colore = string.Empty;
            string query = "SELECT CODESCOL FROM [" + schema + "].[dbo].[" + azienda + "TCCOLORI] WHERE COCODCOL = '" + CodiceColore + "'";
            SqlCommand command = new SqlCommand(query, connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    Colore = reader[0].ToString().Trim();
                }
                reader.Close();
            }
            command.Dispose();
            return Colore;
        }

        public string GetTaglia(string Numerata, string Indice)
        {
            string Taglia = string.Empty;
            var query = "SELECT NUNUME" + Indice + " FROM [" + schema + "].[dbo].[" + azienda + "TCNUMERA] WHERE NUCODNUM = '" + Numerata + "'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Taglia = reader[0].ToString().Trim();
            }
            reader.Close();
            command.Dispose();
            return Taglia;
        }


        public string GetNumerata(string CodiceArticolo)
        {
            string Numerata = string.Empty;
            var query = "SELECT STCODNUM FROM [" + schema + "].[dbo].[" + azienda + "TCSCHMAS] WHERE STCODART = '" + CodiceArticolo + "'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Numerata = reader[0].ToString().Trim();
            }
            reader.Close();
            command.Dispose();
            return Numerata;
        }

        public Articolo GetArticolo(string CodiceArticolo)
        {
            Articolo articolo = new Articolo();
            var query = "SELECT " +
                "" + azienda + "ART_ICOL.ARCODART AS CodiceArticolo, " +
                "" + azienda + "ART_ICOL.ARDESART AS DescrizioneArticolo, " +
                "" + azienda + "ART_ICOL.ARDESSUP AS DescrizioneSupplementareArticolo " +
                "FROM [" + schema + "].[dbo].[" + azienda + "ART_ICOL] " +
                "WHERE ARCODART = '" + CodiceArticolo + "'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                articolo.CodiceArticolo = CodiceArticolo;
                articolo.DescrizioneArticolo = reader["DescrizioneArticolo"].ToString().Trim();
                articolo.DescrizioneSupplementareArticolo = reader["DescrizioneSupplementareArticolo"].ToString().Trim();
            }
            reader.Close();
            command.Dispose();
            return articolo;
        }

        public List<NameValueCollection> Get(string table, string[] fields, string[] where)
        {
            List<NameValueCollection> objects = new List<NameValueCollection>();
            var query = "SELECT * FROM [" + schema + "].[dbo].[" + table + "] WHERE " + string.Join(" AND ", where);
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                NameValueCollection obj = new NameValueCollection();
                for (int i = 0; i < fields.Length; i++)
                {
                    obj.Add(fields[i], reader[fields[i]]?.ToString().Trim());
                }
                objects.Add(obj);
            }
            reader.Close();
            command.Dispose();
            return objects;
        }

        public Articolo FromBarcode(string Barcode)
        {
            Articolo articolo = null;
            var query = "SELECT " +
                "" + azienda + "KEY_ARTI.CACODICE AS Barcode, " +
                "" + azienda + "ART_ICOL.ARCODART AS CodiceArticolo, " +
                "" + azienda + "ART_ICOL.ARDESART AS DescrizioneArticolo, " +
                "" + azienda + "ART_ICOL.ARDESSUP AS DescrizioneSupplementareArticolo, " +
                "" + azienda + "KEY_ARTI.TCCOLORE AS CodiceColore, " +
                "" + azienda + "TCCOLORI.CODESCOL AS DescrizioneColore, " +
                "" + azienda + "TCNUMERA.NUCODNUM AS CodiceNumerata, " +
                "" + azienda + "TCNUMERA.NUDESNUM AS DescrizioneNumerata, " +
                "" + azienda + "KEY_ARTI.TCTAGLIA AS CodiceTaglia, " +
                "" + azienda + "ART_ICOL.TCMADEIN AS CodiceMadeIn, " +
                "" + azienda + "ART_ICOL.ARCODMAR AS CodiceMarca, " +
                "" + azienda + "ART_ICOL.ARSTAGIO AS CodiceStagione, " +
                "" + azienda + "ART_ICOL.TCGENERE AS CodiceGenere, " +
                "" + azienda + "ART_ICOL.ARGRUMER AS CodiceGruppoMerceologico, " +
                "" + azienda + "ART_ICOL.ARCATOMO AS CodiceCategoriaOmogenea, " +
                "" + azienda + "TCSCHMAS.STCODFOR AS CodiceFornitore, " +
                "" + azienda + "KEY_ARTI.CATIPBAR AS CodificaBarcode, " +
                "" + azienda + "KEY_ARTI.CATIPCON AS TipoBarcode " +
                "FROM [" + schema + "].[dbo].[" + azienda + "KEY_ARTI] " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "ART_ICOL] ON " + azienda + "KEY_ARTI.CACODART = " + azienda + "ART_ICOL.ARCODART " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "TCCOLORI] ON " + azienda + "TCCOLORI.COCODCOL = " + azienda + "KEY_ARTI.TCCOLORE " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "TCSCHMAS] ON " + azienda + "TCSCHMAS.STCODART = " + azienda + "KEY_ARTI.CACODART " +
                "INNER JOIN [" + schema + "].[dbo].[" + azienda + "TCNUMERA] ON " + azienda + "TCNUMERA.NUCODNUM = " + azienda + "TCSCHMAS.STCODNUM " +
                "WHERE CACODICE = '" + Barcode + "'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                articolo = new Articolo
                {
                    Barcode = Barcode,
                    CodiceArticolo = reader["CodiceArticolo"].ToString().Trim(),
                    DescrizioneArticolo = reader["DescrizioneArticolo"].ToString().Trim(),
                    DescrizioneSupplementareArticolo = reader["DescrizioneSupplementareArticolo"].ToString().Trim(),
                    CodiceColore = reader["CodiceColore"].ToString().Trim(),
                    DescrizioneColore = reader["DescrizioneColore"].ToString().Trim(),
                    CodiceTaglia = reader["CodiceTaglia"].ToString().Trim(),
                    CodiceNumerata = reader["CodiceNumerata"].ToString().Trim(),
                    DescrizioneNumerata = reader["DescrizioneNumerata"].ToString().Trim()
                };
                articolo.Prezzo = GetPrezzo(articolo.CodiceArticolo, "VENP");
                articolo.CodificaBarcode = reader["CodificaBarcode"].ToString().Trim();
                articolo.TipoBarcode = reader["TipoBarcode"].ToString().Trim();
                articolo.CodiceMadeIn = reader["CodiceMadeIn"].ToString().Trim();
                articolo.CodiceStagione = reader["CodiceStagione"].ToString().Trim();
                articolo.CodiceGenere = reader["CodiceGenere"].ToString().Trim();
                articolo.CodiceGruppoMerceologico = reader["CodiceGruppoMerceologico"].ToString().Trim();
                articolo.CodiceCategoriaOmogenea = reader["CodiceCategoriaOmogenea"].ToString().Trim();
                articolo.Info = GetArticoloInfo(articolo.CodiceArticolo, articolo.CodiceColore, articolo.CodiceTaglia);
                articolo.FuoriProduzione = articolo.Info.FuoriProduzione ? "Si" : "No";
                List<string> Taglie = GetTaglie(articolo.CodiceNumerata);
                string IndiceTaglia = (Taglie.IndexOf(articolo.CodiceTaglia) + 1).ToString().PadLeft(2, '0');
                articolo.Quantita = GetQuantita(articolo.CodiceArticolo, articolo.CodiceColore, IndiceTaglia).ToString();
            }
            reader.Close();
            command.Dispose();
            return articolo;
        }

        public bool CheckExists(string table, string[] fields, string[] values, string schema)
        {
            List<string> where = new List<string>();
            foreach (var field in fields)
            {
                where.Add(field + " = @" + field);
            }
            var query = "SELECT * FROM [" + schema + "].[dbo].[" + table + "] WHERE " + string.Join(" AND ", where);
            SqlCommand command = new SqlCommand(query, connection);
            for (int i = 0; i < fields.Length; i++)
            {
                command.Parameters.Add(new SqlParameter(fields[i], values[i]));
            }
            SqlDataReader reader = command.ExecuteReader();
            bool exists = reader.HasRows;
            reader.Close();
            command.Dispose();
            return exists;
        }
    }
}
