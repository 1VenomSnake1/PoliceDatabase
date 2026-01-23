using Microsoft.Extensions.DependencyInjection;
using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel) // Конструктор с 1 параметром
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Используем DI для создания LoginWindow
            var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
            this.Close();
        }
    }
}