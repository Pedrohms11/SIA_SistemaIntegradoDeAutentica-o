using Microsoft.Data.Sqlite;
using System.IO;
using System.Windows;
namespace InterfaceDeAutenticação.Data
{
    public static class Database
    {
        private static readonly string pastaBase =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SistemaDeAutenticacao"
            );

        private static readonly string caminhoBanco =
            Path.Combine(pastaBase, "SistemaDeAutenticacao.db");

        private static readonly string connectionString =
            $"Data Source={caminhoBanco}";

        static Database()
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
