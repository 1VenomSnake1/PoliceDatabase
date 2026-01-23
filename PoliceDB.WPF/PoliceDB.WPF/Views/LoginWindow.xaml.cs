using PoliceDB.BLL.Services;
using PoliceDB.WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PoliceDB.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _loginViewModel;

        public LoginWindow()
        {
            InitializeComponent();

            // Создаем ViewModel
            _loginViewModel = new LoginViewModel(new MockAuthService());
            DataContext = _loginViewModel;

            // Подписываемся на события
            _loginViewModel.LoginSuccessful += OnLoginSuccessful;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_loginViewModel != null)
            {
                _loginViewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Обновляем состояние кнопки
            if (_loginViewModel != null)
            {
                _loginViewModel.LoginCommand.NotifyCanExecuteChanged();
            }
        }

        private void OnLoginSuccessful(object? sender, PoliceDB.Core.Models.User user)
        {
            // Получаем caseId из ViewModel
            string caseId = _loginViewModel.CaseId;

            // Создаем главное окно с данными пользователя и дела
            var mainWindow = new MainWindow(user, caseId);
            mainWindow.Show();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_loginViewModel != null)
            {
                _loginViewModel.LoginSuccessful -= OnLoginSuccessful;
            }
        }
    }
}