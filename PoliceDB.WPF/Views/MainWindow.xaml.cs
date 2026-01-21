using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Простое создание окна авторизации
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}