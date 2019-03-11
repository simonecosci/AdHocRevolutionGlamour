using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdHocRevolutionGlamour
{
    public class ArticoloInfo
    {
        public string CodiceArticolo;
        public string CodiceColore;
        public string CodiceTaglia;
        public string Numerata;
        public bool FuoriProduzione;

        public static Connection connection;
        public static string schema_info;

        public static void Save(ArticoloInfo articolo)
        {
            if (!connection.CheckExists("ARTICOLI", new string[] { "CodiceArticolo", "CodiceTaglia", "CodiceColore" }, new string[] { articolo.CodiceArticolo, articolo.CodiceTaglia, articolo.CodiceColore }, schema_info))
            {
                string sql = "INSERT INTO [" + schema_info + "].[dbo].[ARTICOLI] (CodiceArticolo, CodiceTaglia, CodiceColore, Numerata, FuoriProduzione) VALUES (@CodiceArticolo, @CodiceTaglia, @CodiceColore, @Numerata, @FuoriProduzione)";
                SqlCommand command = new SqlCommand(sql, connection.connection);
                command.Parameters.AddWithValue("CodiceArticolo", articolo.CodiceArticolo);
                command.Parameters.AddWithValue("CodiceTaglia", articolo.CodiceTaglia);
                command.Parameters.AddWithValue("CodiceColore", articolo.CodiceColore);
                command.Parameters.AddWithValue("Numerata", articolo.Numerata);
                command.Parameters.AddWithValue("FuoriProduzione", articolo.FuoriProduzione);
                command.ExecuteNonQuery();
                command.Dispose();
            }
            else
            {
                string sql = "UPDATE [" + schema_info + "].[dbo].[ARTICOLI] SET " +
                    "Numerata = @Numerata, " +
                    "FuoriProduzione = @FuoriProduzione " +
                    "WHERE " +
                    "CodiceArticolo = @CodiceArticolo AND " +
                    "CodiceTaglia = @CodiceTaglia AND " +
                    "CodiceColore = @CodiceColore";
                SqlCommand command = new SqlCommand(sql, connection.connection);
                command.Parameters.AddWithValue("CodiceArticolo", articolo.CodiceArticolo);
                command.Parameters.AddWithValue("CodiceTaglia", articolo.CodiceTaglia);
                command.Parameters.AddWithValue("CodiceColore", articolo.CodiceColore);
                command.Parameters.AddWithValue("Numerata", articolo.Numerata);
                command.Parameters.AddWithValue("FuoriProduzione", articolo.FuoriProduzione);
                command.ExecuteNonQuery();
                command.Dispose();
            }
        }
    }
}
