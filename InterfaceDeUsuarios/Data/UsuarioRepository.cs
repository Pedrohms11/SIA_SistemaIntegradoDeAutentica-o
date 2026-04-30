using Microsoft.Data.Sqlite;
using SIA_SistemaIntegradoDeAutenticação;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace InterfaceDeUsuarios.Data
{
    public class UsuarioRepository
    {
        public void Inserir(Usuarios usuarios)
        {
            using var conn = DataBase.GetConnection();
            conn.Open();

            var cmd = new SqliteCommand(@"
    INSERT INTO Usuarios 
    (Username, NomeCompleto, Email, Senha, Genero, Telefone, Pais, 
     DataNascimento, DataCadastro, UltimoLogin, EmailVerificado)
    VALUES (@username, @nomeCompleto, @email, @senha, @genero, @telefone, @pais, 
            @dataNascimento, @dataCadastro, @ultimoLogin, @emailVerificado)", conn);

            cmd.Parameters.AddWithValue("@username", usuarios.Username);
            cmd.Parameters.AddWithValue("@nomeCompleto", usuarios.NomeCompleto);
            cmd.Parameters.AddWithValue("@email", usuarios.Email);
            cmd.Parameters.AddWithValue("@senha", usuarios.Senha);
            cmd.Parameters.AddWithValue("@genero", usuarios.Genero ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@telefone", usuarios.Telefone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@pais", usuarios.Pais ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@dataNascimento", usuarios.DataNascimento);
            cmd.Parameters.AddWithValue("@dataCadastro", usuarios.DataCadastro);
            cmd.Parameters.AddWithValue("@ultimoLogin", usuarios.UltimoLogin);
            cmd.Parameters.AddWithValue("@emailVerificado", usuarios.EmailVerificado ? 1 : 0);

            cmd.ExecuteNonQuery();

            if (usuarios.Id == 0)
            {
                cmd.CommandText = "SELECT last_insertrowid()";
                usuarios.Id = Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public List<Usuarios> ListarTodos()
        {
            var usuarios = new List<Usuarios>();

            using var conn = DataBase.GetConnection();
            conn.Open();

            var cmd = new SqliteCommand(@"
    SELECT Id, Username, NomeCompleto, Email, Senha, Genero, 
           Telefone, Pais, DataNascimento, DataCadastro, 
           UltimoLogin, EmailVerificado
    FROM Usuarios 
    ORDER BY NomeCompleto", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var usuario = new Usuarios
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    NomeCompleto = reader.GetString(2),
                    Email = reader.GetString(3),
                    Senha = reader.GetString(4),
                    Genero = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Telefone = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Pais = reader.IsDBNull(7) ? null : reader.GetString(7),
                    DataNascimento = reader.GetDateTime(8),
                    DataCadastro = reader.GetDateTime(9),
                    UltimoLogin = reader.GetDateTime(10),
                    EmailVerificado = reader.GetBoolean(11)
                };
                usuarios.Add(usuario);
            }

            return usuarios;
        }
        public Usuarios BuscarPorId(int id)
        {
            using var cmd = new SqliteCommand(@"SELECT Id, Username, NomeCompleto, Email, Senha, Genero, 
                                               Telefone, Pais, DataNascimento, DataCadastro, 
                                               UltimoLogin, EmailVerificado
                                               FROM Usuarios 
                                               WHERE Id = @id", conn);

            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                var usuario = new Usuarios
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    NomeCompleto = reader.GetString(2),
                    Email = reader.GetString(3),
                    Senha = reader.GetString(4),
                    Genero = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Telefone = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Pais = reader.IsDBNull(7) ? null : reader.GetString(7),
                    DataNascimento = reader.GetDateTime(8),
                    DataCadastro = reader.GetDateTime(9),
                    UltimoLogin = reader.GetDateTime(10),
                    EmailVerificado = reader.GetBoolean(11)
                };

                if (usuario.Id == id)
                {
                    return usuario;
                }
                if (usuario.Id != id)
                {
                    MessageBox.Show("Usuario não encontrado");

                }
            }
        }
        public void Atualizar(Usuarios usuarios)
        {
            using var conn = DataBase.GetConnection();
            conn.Open();
            var cmd = new SqliteCommand(@"
        UPDATE Usuarios
        SET Username = @Username,
            NomeCompleto = @NomeCompleto,
            Email = @Email,
            Senha = @Senha,
            Genero = @Genero,
            Telefone = @Telefone,
            Pais = @Pais,
            DataNascimento = @DataNascimento,
            DataCadastro = @DataCadastro,
            UltimoLogin = @UltimoLogin,
            EmailVerificado = @EmailVerificado
        WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", usuarios.Id);
            cmd.Parameters.AddWithValue("@Username", usuarios.Username);
            cmd.Parameters.AddWithValue("@NomeCompleto", usuarios.NomeCompleto);
            cmd.Parameters.AddWithValue("@Email", usuarios.Email);
            cmd.Parameters.AddWithValue("@Senha", usuarios.Senha);
            cmd.Parameters.AddWithValue("@Genero", usuarios.Genero ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefone", usuarios.Telefone ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Pais", usuarios.Pais ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DataNascimento", usuarios.DataNascimento);
            cmd.Parameters.AddWithValue("@DataCadastro", usuarios.DataCadastro);
            cmd.Parameters.AddWithValue("@UltimoLogin", usuarios.UltimoLogin);
            cmd.Parameters.AddWithValue("@EmailVerificado", usuarios.EmailVerificado);

            cmd.ExecuteNonQuery();
        }
        public void excluir(int id)
        {
            using var conn = DataBase.GetConnection();
            conn.Open();
            var cmd = new SqliteCommand("DELETE FROM Usuarios WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
