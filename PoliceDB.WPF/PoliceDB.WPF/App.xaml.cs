using PoliceDB.WPF.Views;
using System.Windows;

namespace PoliceDB.WPF
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Просто создаем и показываем окно авторизации
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}