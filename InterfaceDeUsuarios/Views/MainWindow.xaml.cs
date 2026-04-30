using System.Windows;
using InterfaceDeUsuarios.ViewModel;

namespace InterfaceDeUsuarios
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Instancia o ViewModel e define como DataContext
                var viewModel = new UsuarioViewModel();
                this.DataContext = viewModel;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erro ao inicializar a aplicação: {ex.Message}\n\nDetalhes: {ex.StackTrace}",
                    "Erro Crítico",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}