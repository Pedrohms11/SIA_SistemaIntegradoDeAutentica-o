using InterfaceDeUsuarios.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InterfaceDeUsuarios
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Conectar PasswordBox com ViewModel
            var viewModel = (UsuarioViewModel)this.Resources["ViewModel"];

            // Evento para sincronizar senhas
            this.Loaded += (s, e) =>
            {
                var txtNovaSenha = (System.Windows.Controls.PasswordBox)this.FindName("txtNovaSenha");
                var txtConfirmarSenha = (System.Windows.Controls.PasswordBox)this.FindName("txtConfirmarSenha");

                txtNovaSenha.PasswordChanged += (sender, args) =>
                {
                    viewModel.NovaSenha = txtNovaSenha.Password;
                };

                txtConfirmarSenha.PasswordChanged += (sender, args) =>
                {
                    viewModel.ConfirmarSenha = txtConfirmarSenha.Password;
                };
            };
        }
    }
}
