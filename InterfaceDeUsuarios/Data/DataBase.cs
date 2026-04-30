using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InterfaceDeUsuarios.Data
{
    public class DataBase
    {
        private static readonly string pastaBase =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "InterfaceDeUsuarios"
            );

        private static readonly string caminhoBanco =
            Path.Combine(pastaBase, "InterfaceUs.db");

        private static readonly string connectionString =
            $"Data Source={caminhoBanco}";

         static DataBase()
        {
            if (!Directory.Exists(pastaBase))
                Directory.CreateDirectory(pastaBase);

            if (!File.Exists(caminhoBanco))
                MessageBox.Show("Banco de dados inexistente !!!");
        }

        public static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }
    }
}

