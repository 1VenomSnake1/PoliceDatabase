using PoliceDB.BLL.Services;
using PoliceDB.WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PoliceDB.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            _viewModel = new LoginViewModel(new MockAuthService());
            DataContext = _viewModel;

            _viewModel.LoginSuccessful += OnLoginSuccessful;

            UpdateUIForRole("Следователь");
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = ((PasswordBox)sender).Password;
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var role = selectedItem.Content.ToString();
                UpdateUIForRole(role);

                _viewModel.LoginCommand.NotifyCanExecuteChanged();
            }
        }

        private void UpdateUIForRole(string role)
        {
            Console.WriteLine($"UpdateUIForRole called with role: {role}");

            if (role == "Следователь" || role == "Старший следователь")
            {
                Console.WriteLine("Showing DepartmentPanel");
                DepartmentPanel.Visibility = Visibility.Visible;
                DepartmentTextBox.IsEnabled = true;
            }
            else
            {
                Console.WriteLine("Hiding DepartmentPanel");
                DepartmentPanel.Visibility = Visibility.Collapsed;
                DepartmentTextBox.Clear();
                DepartmentTextBox.IsEnabled = false;
            }

            // Принудительно обновляем layout
            DepartmentPanel.UpdateLayout();
        }

        private void OnLoginSuccessful(object? sender, PoliceDB.Core.Models.User user)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}