using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MongoDB.Driver;
using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using System;
using System.Windows;

namespace PoliceDB.WPF.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _username = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _password = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _caseId = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _departmentNumber = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string _selectedRole = "Следователь";

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public event EventHandler<User>? LoginSuccessful;

        public IRelayCommand LoginCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);

        }

        

        private bool CanExecuteLogin()
        {
            // Сбрасываем ошибку при каждом вызове
            HasError = false;
            ErrorMessage = string.Empty;

            bool isSearcherRole = SelectedRole == "Следователь" || SelectedRole == "Старший следователь";

            // Проверяем обязательные поля для всех ролей
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(CaseId))
            {
                return false;
            }

            // Проверяем номер участка для следователей
            if (isSearcherRole && string.IsNullOrWhiteSpace(DepartmentNumber))
            {
                return false;
            }

            return true;
        }

        private void ExecuteLogin()
        {
            try
            {
                // Сбрасываем ошибку
                HasError = false;
                ErrorMessage = string.Empty;

                // Преобразуем строку роли в enum
                UserRole role = SelectedRole switch
                {
                    "Следователь" => UserRole.Investigator,
                    "Старший следователь" => UserRole.SeniorInvestigator,
                    "Присяжный" => UserRole.Juror,
                    "Адвокат/Прокурор" => UserRole.LawyerProsecutor,
                    "Судья" => UserRole.Judge,
                    "Администратор" => UserRole.Administrator,
                    _ => throw new ArgumentException("Неизвестная роль")
                };

                // Вызываем реальный сервис аутентификации
                var user = _authService.Login(Username, Password, CaseId, role, DepartmentNumber);

                if (user != null)
                {
                    // Успешный вход
                    LoginSuccessful?.Invoke(this, user);
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Неверные учетные данные или отсутствует доступ к делу";
                }
            }
            catch (ArgumentException ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Ошибка авторизации: {ex.Message}";
            }
        }
    }
}