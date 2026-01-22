using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // Временные данные для тестирования UI
            var mockUser = new User
            {
                Id = "1",
                Username = "investigator",
                Role = UserRole.Investigator,
                DepartmentNumber = "42"
            };

            _viewModel = new MainViewModel(mockUser, "CASE-001");
            DataContext = _viewModel;
        }

        // Конструктор с параметрами для передачи реальных данных
        public MainWindow(User user, string caseId)
        {
            InitializeComponent();

            _viewModel = new MainViewModel(user, caseId);
            DataContext = _viewModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}