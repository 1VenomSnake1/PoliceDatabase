using Microsoft.Extensions.DependencyInjection;
using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PoliceDB.WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _loginViewModel;

        // Конструктор с параметрами (как было изначально)
        public LoginWindow(IAuthService authService, LoginViewModel loginViewModel)
        {
            InitializeComponent();

            _loginViewModel = loginViewModel;
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

            // Получаем MainViewModel через DI
            var mainViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();

            // Устанавливаем данные пользователя и дела
            mainViewModel.SetUserData(user, caseId);

            // Создаем главное окно с ViewModel
            var mainWindow = new MainWindow(mainViewModel);
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