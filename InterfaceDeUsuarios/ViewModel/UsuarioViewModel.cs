using InterfaceDeUsuarios.Commands;
using SIA_SistemaIntegradoDeAutenticação;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InterfaceDeUsuarios.ViewModel
{
    public class UsuarioViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Usuarios> _usuarios;
        private Usuarios _usuarioSelecionado;
        private string _termoBusca;
        private int _tipoBusca;
        private string _novaSenha;
        private string _confirmarSenha;

        public event PropertyChangedEventHandler PropertyChanged;

        public UsuarioViewModel()
        {
            _context = new AppDbContext();
            CarregarUsuariosCommand = new RelayCommand(CarregarUsuarios);
            BuscarCommand = new RelayCommand(BuscarUsuarios);
            SalvarCommand = new RelayCommand(SalvarUsuario, CanSalvar);
            NovoCommand = new RelayCommand(NovoUsuario);
            LimparCommand = new RelayCommand(LimparFormulario);

            CarregarUsuarios();
        }

        // Propriedades
        public ObservableCollection<Usuarios> Usuarios
        {
            get => _usuarios;
            set
            {
                _usuarios = value;
                OnPropertyChanged();
            }
        }

        public Usuarios UsuarioSelecionado
        {
            get => _usuarioSelecionado;
            set
            {
                _usuarioSelecionado = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NovaSenha = "";
                    ConfirmarSenha = "";
                }
                (SalvarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string TermoBusca
        {
            get => _termoBusca;
            set
            {
                _termoBusca = value;
                OnPropertyChanged();
            }
        }

        public int TipoBusca
        {
            get => _tipoBusca;
            set
            {
                _tipoBusca = value;
                OnPropertyChanged();
            }
        }

        public string NovaSenha
        {
            get => _novaSenha;
            set
            {
                _novaSenha = value;
                OnPropertyChanged();
                (SalvarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string ConfirmarSenha
        {
            get => _confirmarSenha;
            set
            {
                _confirmarSenha = value;
                OnPropertyChanged();
                (SalvarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        // Listas para ComboBox
        public ObservableCollection<string> ListaGeneros { get; } = new ObservableCollection<string>
        {
            "Masculino",
            "Feminino",
            "Outro",
            "Prefiro não informar"
        };

        public ObservableCollection<string> ListaPaises { get; } = new ObservableCollection<string>
        {
            "Brasil",
            "Portugal",
            "Estados Unidos",
            "Canadá",
            "Inglaterra",
            "França",
            "Alemanha",
            "Espanha",
            "Itália",
            "Japão",
            "Austrália"
        };

        public ObservableCollection<string> TiposBusca { get; } = new ObservableCollection<string>
        {
            "Nome Completo",
            "Email",
            "Username",
            "ID"
        };

        // Commands
        public ICommand CarregarUsuariosCommand { get; }
        public ICommand BuscarCommand { get; }
        public ICommand SalvarCommand { get; }
        public ICommand NovoCommand { get; }
        public ICommand LimparCommand { get; }

        // Métodos
        private void CarregarUsuarios()
        {
            try
            {
                Usuarios = new ObservableCollection<Usuarios>(
                    _context.Usuarios.OrderBy(u => u.NomeCompleto).ToList()
                );
                MessageBox.Show($"✅ {Usuarios.Count} usuários carregados!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuscarUsuarios()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TermoBusca))
                {
                    CarregarUsuarios();
                    return;
                }

                IQueryable<Usuarios> query = _context.Usuarios;

                switch (TipoBusca)
                {
                    case 0: // Nome Completo
                        query = query.Where(u => u.NomeCompleto.Contains(TermoBusca));
                        break;
                    case 1: // Email
                        query = query.Where(u => u.Email.Contains(TermoBusca));
                        break;
                    case 2: // Username
                        query = query.Where(u => u.Username.Contains(TermoBusca));
                        break;
                    case 3: // ID
                        if (int.TryParse(TermoBusca, out int id))
                            query = query.Where(u => u.Id == id);
                        break;
                }

                var resultado = query.OrderBy(u => u.NomeCompleto).ToList();
                Usuarios = new ObservableCollection<Usuarios>(resultado);

                MessageBox.Show($"🔍 Encontrados {resultado.Count} usuário(s)", "Busca",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na busca: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SalvarUsuario()
        {
            try
            {
                if (UsuarioSelecionado == null)
                {
                    MessageBox.Show("Selecione um usuário para atualizar!", "Aviso",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validações
                if (!ValidarUsuario())
                    return;

                // Buscar o usuário original no contexto
                var usuarioOriginal = _context.Usuarios.Find(UsuarioSelecionado.Id);
                if (usuarioOriginal == null)
                {
                    MessageBox.Show("Usuário não encontrado no banco de dados!", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Atualizar apenas os campos editáveis
                usuarioOriginal.NomeCompleto = UsuarioSelecionado.NomeCompleto;
                usuarioOriginal.Email = UsuarioSelecionado.Email;
                usuarioOriginal.Telefone = UsuarioSelecionado.Telefone;
                usuarioOriginal.Genero = UsuarioSelecionado.Genero;
                usuarioOriginal.Pais = UsuarioSelecionado.Pais;
                usuarioOriginal.DataNascimento = UsuarioSelecionado.DataNascimento;
                usuarioOriginal.EmailVerificado = UsuarioSelecionado.EmailVerificado;

                // Atualizar senha se foi alterada
                if (!string.IsNullOrEmpty(NovaSenha))
                {
                    if (NovaSenha != ConfirmarSenha)
                    {
                        MessageBox.Show("As senhas não conferem!", "Erro",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    usuarioOriginal.Senha = NovaSenha;
                }

                // Confirmar salvamento
                var result = MessageBox.Show($"Deseja salvar as alterações para {usuarioOriginal.NomeCompleto}?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                _context.SaveChanges();

                MessageBox.Show($"✅ Usuário {usuarioOriginal.NomeCompleto} atualizado com sucesso!",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                // Recarregar lista
                CarregarUsuarios();

                // Encontrar e selecionar o usuário atualizado
                UsuarioSelecionado = Usuarios.FirstOrDefault(u => u.Id == usuarioOriginal.Id);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.InnerException?.Message ?? ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarUsuario()
        {
            if (string.IsNullOrWhiteSpace(UsuarioSelecionado.NomeCompleto))
            {
                MessageBox.Show("Nome completo é obrigatório!", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(UsuarioSelecionado.Email))
            {
                MessageBox.Show("Email é obrigatório!", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!UsuarioSelecionado.Email.Contains("@") || !UsuarioSelecionado.Email.Contains("."))
            {
                MessageBox.Show("Email inválido!", "Validação",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Validar telefone se foi preenchido
            if (!string.IsNullOrWhiteSpace(UsuarioSelecionado.Telefone))
            {
                var telefoneLimpo = new string(UsuarioSelecionado.Telefone.Where(char.IsDigit).ToArray());
                if (telefoneLimpo.Length < 10 || telefoneLimpo.Length > 11)
                {
                    MessageBox.Show("Telefone inválido! Use formato (DD) 99999-9999", "Validação",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private bool CanSalvar()
        {
            return UsuarioSelecionado != null;
        }

        private void NovoUsuario()
        {
            UsuarioSelecionado = new Usuarios
            {
                DataCadastro = DateTime.Now,
                UltimoLogin = DateTime.Now,
                EmailVerificado = false,
                Genero = "Masculino",
                Pais = "Brasil",
                DataNascimento = new DateTime(2000, 1, 1)
            };
            NovaSenha = "";
            ConfirmarSenha = "";
        }

        private void LimparFormulario()
        {
            UsuarioSelecionado = null;
            TermoBusca = "";
            TipoBusca = 0;
            NovaSenha = "";
            ConfirmarSenha = "";
            CarregarUsuarios();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
