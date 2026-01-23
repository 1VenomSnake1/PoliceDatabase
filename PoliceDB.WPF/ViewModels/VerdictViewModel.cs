using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliceDB.BLL.Interfaces;
using PoliceDB.BLL.Services;
using PoliceDB.Core.Models;
using System;
using System.Windows;

namespace PoliceDB.WPF.ViewModels
{
    public partial class VerdictViewModel : ObservableObject
    {
        private readonly ICaseService _caseService;
        private readonly string _caseId;
        private readonly User _currentUser;
        private readonly Window _window;
        private Case _currentCase;

        [ObservableProperty]
        private string _caseTitle = string.Empty;

        [ObservableProperty]
        private string _caseIdText = string.Empty;

        [ObservableProperty]
        private bool _isClosed = false;

        [ObservableProperty]
        private string _statusText = string.Empty;

        [ObservableProperty]
        private string _currentStatusText = string.Empty;

        [ObservableProperty]
        private string _verdictText = string.Empty;

        [ObservableProperty]
        private DateTime? _verdictDate = null;

        [ObservableProperty]
        private string _statusColor = "#4CAF50";

        [ObservableProperty]
        private string _currentStatusColor = "#FFC107";

        [ObservableProperty]
        private string _judgeName = string.Empty;

        [ObservableProperty]
        private DateTime _caseCreatedDate = DateTime.Now;

        [ObservableProperty]
        private int _protocolsCount = 0;

        // Вычисляемые свойства для UI
        public string FormattedCaseCreatedDate => CaseCreatedDate.ToString("dd.MM.yyyy");
        public string ProtocolsCountText => $"Протоколов: {ProtocolsCount}";
        public string VerdictDateText => VerdictDate.HasValue
            ? $"Дата вынесения приговора: {VerdictDate.Value:dd.MM.yyyy HH:mm}"
            : string.Empty;

        // Команды
        public IRelayCommand GuiltyCommand { get; }
        public IRelayCommand NotGuiltyCommand { get; }
        public IRelayCommand BackCommand { get; }

        public VerdictViewModel(Window window, string caseId, User currentUser)
        {
            _window = window;
            _caseId = caseId;
            _currentUser = currentUser;

            // Используем Mock сервис
            _caseService = new MockCaseService();

            GuiltyCommand = new RelayCommand(GiveGuiltyVerdict, CanGiveVerdict);
            NotGuiltyCommand = new RelayCommand(GiveNotGuiltyVerdict, CanGiveVerdict);
            BackCommand = new RelayCommand(Back);

            LoadCaseData();
        }

        private void LoadCaseData()
        {
            try
            {
                // Загружаем дело
                _currentCase = _caseService.GetCase(_caseId);
                if (_currentCase == null)
                {
                    MessageBox.Show("Дело не найдено", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    _window.Close();
                    return;
                }

                // Устанавливаем свойства
                CaseTitle = _currentCase.Title;
                CaseIdText = $"Номер дела: {_currentCase.Id}";
                CaseCreatedDate = _currentCase.CreatedDate;
                ProtocolsCount = _currentCase.Protocols?.Count ?? 0;
                JudgeName = _currentUser.Username;

                // Определяем, закрыто ли дело
                IsClosed = _currentCase.Status == CaseStatus.ClosedGuilty ||
                          _currentCase.Status == CaseStatus.ClosedNotGuilty;

                // Устанавливаем цвета и тексты статусов
                SetStatusProperties();

                // Если дело уже закрыто, устанавливаем текст вердикта
                if (IsClosed && !string.IsNullOrEmpty(_currentCase.Verdict))
                {
                    VerdictText = _currentCase.Verdict;
                    VerdictDate = _currentCase.VerdictDate;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных дела: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _window.Close();
            }
        }

        private void SetStatusProperties()
        {
            string statusText = string.Empty;
            string currentStatusText = string.Empty;
            string statusColor = "#4CAF50";
            string currentStatusColor = "#FFC107";

            switch (_currentCase.Status)
            {
                case CaseStatus.Open:
                    statusText = "Дело открыто";
                    currentStatusText = "Открыто";
                    statusColor = "#4CAF50";
                    currentStatusColor = "#4CAF50";
                    break;

                case CaseStatus.Investigation:
                    statusText = "В процессе расследования";
                    currentStatusText = "Расследование";
                    statusColor = "#FFC107";
                    currentStatusColor = "#FFC107";
                    break;

                case CaseStatus.Trial:
                    statusText = "Судебное разбирательство";
                    currentStatusText = "Судебное разбирательство";
                    statusColor = "#FFC107";
                    currentStatusColor = "#FFC107";
                    break;

                case CaseStatus.Closed:
                    statusText = "Дело закрыто";
                    currentStatusText = "Закрыто";
                    statusColor = "#F44336";
                    currentStatusColor = "#F44336";
                    break;

                case CaseStatus.ClosedGuilty:
                    statusText = "Дело закрыто: ВИНОВЕН";
                    currentStatusText = "ВИНОВЕН";
                    statusColor = "#F44336";
                    currentStatusColor = "#F44336";
                    break;

                case CaseStatus.ClosedNotGuilty:
                    statusText = "Дело закрыто: НЕ ВИНОВЕН";
                    currentStatusText = "НЕ ВИНОВЕН";
                    statusColor = "#4CAF50";
                    currentStatusColor = "#4CAF50";
                    break;
            }

            StatusText = statusText;
            CurrentStatusText = currentStatusText;
            StatusColor = statusColor;
            CurrentStatusColor = currentStatusColor;
        }

        private bool CanGiveVerdict()
        {
            // Проверяем, что дело еще не закрыто вердиктом
            return !IsClosed &&
                   _currentCase.Status != CaseStatus.ClosedGuilty &&
                   _currentCase.Status != CaseStatus.ClosedNotGuilty;
        }

        private void GiveGuiltyVerdict()
        {
            GiveVerdict("ВИНОВЕН", CaseStatus.ClosedGuilty);
        }

        private void GiveNotGuiltyVerdict()
        {
            GiveVerdict("НЕ ВИНОВЕН", CaseStatus.ClosedNotGuilty);
        }

        private void GiveVerdict(string verdict, CaseStatus status)
        {
            try
            {
                // Запрашиваем подтверждение
                var message = $"Вы уверены, что хотите признать подсудимого {verdict}?\n" +
                             $"После этого дело будет закрыто и статус больше не может быть изменен.";

                var result = MessageBox.Show(message, "Подтверждение приговора",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                // Обновляем статус дела с вердиктом
                bool success = _caseService.UpdateVerdict(_caseId, verdict, status, _currentUser.Id);

                if (!success)
                {
                    MessageBox.Show("Ошибка при сохранении вердикта",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновляем локальную копию дела
                _currentCase = _caseService.GetCase(_caseId);
                if (_currentCase == null)
                {
                    MessageBox.Show("Дело не найдено после сохранения",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновляем UI
                IsClosed = true;
                VerdictText = verdict;
                VerdictDate = _currentCase.VerdictDate;
                SetStatusProperties();

                // Показываем сообщение об успехе
                MessageBox.Show($"Приговор успешно вынесен!\n\n" +
                               $"Вердикт: {verdict}\n" +
                               $"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}\n" +
                               $"Дело закрыто.",
                               "Приговор вынесен",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при вынесении приговора: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back()
        {
            _window.Close();
        }
    }
}