using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using System;
using System.Linq;
using System.Windows;

namespace PoliceDB.WPF.ViewModels
{
    public partial class CaseDetailsViewModel : ObservableObject
    {
        private readonly ICaseService _caseService;
        private readonly User _currentUser;
        private readonly Window _window;
        private readonly Case _currentCase;

        [ObservableProperty]
        private string _caseId = string.Empty;

        [ObservableProperty]
        private string _caseTitle = string.Empty;

        [ObservableProperty]
        private string _caseDescription = string.Empty;

        [ObservableProperty]
        private CaseStatus _caseStatus;

        [ObservableProperty]
        private bool _isEditMode = false;

        [ObservableProperty]
        private bool _canEditDescription = false;

        [ObservableProperty]
        private bool _isViewMode = true;

        [ObservableProperty]
        private string _statusColor = "#4CAF50"; // Зеленый по умолчанию

        [ObservableProperty]
        private string _statusText = "Открыто";

        // Команды
        public IRelayCommand EditDescriptionCommand { get; }
        public IRelayCommand SaveDescriptionCommand { get; }
        public IRelayCommand CancelEditCommand { get; }
        public IRelayCommand BackCommand { get; }

        public CaseDetailsViewModel(Window window, string caseId, User currentUser, ICaseService caseService)
        {
            _window = window;
            _caseId = caseId;
            _currentUser = currentUser;
            _caseService = caseService;

            // Инициализируем сервис
            _caseService = new PoliceDB.BLL.Services.MockCaseService();

            // Получаем дело
            _currentCase = _caseService.GetCase(caseId);

            if (_currentCase == null)
            {
                MessageBox.Show("Дело не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _window.Close();
                return;
            }

            // Загружаем данные
            LoadCaseData();
            DetermineEditPermissions();

            // Инициализируем команды
            EditDescriptionCommand = new RelayCommand(EnableEditMode);
            SaveDescriptionCommand = new RelayCommand(SaveDescription);
            CancelEditCommand = new RelayCommand(CancelEdit);
            BackCommand = new RelayCommand(Back);
        }

        private void LoadCaseData()
        {
            CaseTitle = _currentCase.Title;
            CaseDescription = _currentCase.Description;
            CaseStatus = _currentCase.Status;

            // Устанавливаем цвет и текст статуса
            switch (CaseStatus)
            {
                case CaseStatus.Open:
                    StatusColor = "#4CAF50";
                    StatusText = "Открыто";
                    break;
                case CaseStatus.Investigation:
                    StatusColor = "#FFC107";
                    StatusText = "Расследование";
                    break;
                case CaseStatus.Trial:
                    StatusColor = "#FFC107";
                    StatusText = "Судебное разбирательство";
                    break;
                case CaseStatus.Closed:
                    StatusColor = "#F44336";
                    StatusText = "Закрыто";
                    break;
                case CaseStatus.ClosedGuilty:
                    StatusColor = "#F44336";
                    StatusText = "Закрыто: ВИНОВЕН";
                    break;
                case CaseStatus.ClosedNotGuilty:
                    StatusColor = "#4CAF50";
                    StatusText = "Закрыто: НЕ ВИНОВЕН";
                    break;
            }

            // Добавляем информацию о вердикте, если есть
            if (!string.IsNullOrEmpty(_currentCase.Verdict) && _currentCase.VerdictDate.HasValue)
            {
                StatusText += $"\nВердикт: {_currentCase.Verdict} ({_currentCase.VerdictDate:dd.MM.yyyy})";
            }
        }

        private void DetermineEditPermissions()
        {
            // Проверяем, может ли пользователь редактировать описание
            if (string.IsNullOrEmpty(CaseDescription) || !_currentCase.IsDescriptionInitialized)
            {
                // Если описание пустое или не инициализировано
                // Проверяем роль пользователя
                CanEditDescription = _currentUser.Role == UserRole.Investigator ||
                                    _currentUser.Role == UserRole.SeniorInvestigator ||
                                    _currentUser.Role == UserRole.Administrator;
            }
            else
            {
                // Если описание уже есть, то только просмотр для всех кроме администратора
                // (администратор может редактировать всегда, но это отдельная кнопка)
                CanEditDescription = _currentUser.Role == UserRole.Administrator;
            }
        }

        private void EnableEditMode()
        {
            IsEditMode = true;
            IsViewMode = false;
        }

        private void SaveDescription()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CaseDescription))
                {
                    MessageBox.Show("Описание не может быть пустым", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверяем, нужно ли отправлять на одобрение администратора
                if (_currentUser.Role == UserRole.SeniorInvestigator && _currentCase.IsDescriptionInitialized)
                {
                    // Для старшего следователя изменения требуют одобрения администратора
                    // Создаем запись в PendingChange
                    // TODO: Реализовать сохранение через PendingChange
                    MessageBox.Show("Изменения отправлены на рассмотрение администратору",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Для следователя (при первом создании) или администратора сохраняем сразу
                    _currentCase.Description = CaseDescription;
                    _currentCase.IsDescriptionInitialized = true;

                    // Обновляем в сервисе
                    _caseService.UpdateCaseDescription(_caseId, CaseDescription, _currentUser.Id);

                    MessageBox.Show("Описание успешно сохранено", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Выходим из режима редактирования
                IsEditMode = false;
                IsViewMode = true;
                CanEditDescription = false; // После сохранения запрещаем редактирование

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit()
        {
            if (!string.IsNullOrEmpty(_currentCase.Description))
            {
                // Восстанавливаем оригинальное описание
                CaseDescription = _currentCase.Description;
            }

            IsEditMode = false;
            IsViewMode = true;
        }

        private void Back()
        {
            _window.Close();
        }
    }
}