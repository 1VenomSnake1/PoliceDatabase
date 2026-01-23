using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliceDB.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MenuItemModel = PoliceDB.WPF.Models.MenuItem;
using PoliceDB.WPF.Views;

namespace PoliceDB.WPF.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly User _currentUser;
        private readonly string _caseId;

        [ObservableProperty]
        private string _caseInfo = string.Empty;

        [ObservableProperty]
        private string _userInfo = string.Empty;

        [ObservableProperty]
        private ObservableCollection<PoliceDB.WPF.Models.MenuItem> _menuItems = new();

        public MainViewModel(User user, string caseId)
        {
            _currentUser = user;
            _caseId = caseId;

            InitializeUserInfo();
            InitializeMenuItems();
        }

        private void InitializeUserInfo()
        {
            CaseInfo = $"Дело: {_caseId}";

            string roleName = _currentUser.Role switch
            {
                UserRole.Investigator => "Следователь",
                UserRole.SeniorInvestigator => "Старший следователь",
                UserRole.Juror => "Присяжный",
                UserRole.LawyerProsecutor => "Адвокат/Прокурор",
                UserRole.Judge => "Судья",
                UserRole.Administrator => "Администратор",
                _ => "Неизвестная роль"
            };

            UserInfo = $"{roleName} | {_currentUser.Username}";

            if (!string.IsNullOrEmpty(_currentUser.DepartmentNumber))
            {
                UserInfo += $" | Участок: {_currentUser.DepartmentNumber}";
            }
        }

        private void InitializeMenuItems()
        {
            // Создаем все возможные пункты меню
            var allMenuItems = new System.Collections.Generic.List<PoliceDB.WPF.Models.MenuItem>
            {
                // Добавление улики
                new PoliceDB.WPF.Models.MenuItem
                {
                    Title = "Добавить улику",
                    Description = "Добавить новую улику в дело",
                    Icon = "🔍", // Лупа
                    AllowedRoles = new[] { UserRole.Investigator, UserRole.Administrator },
                    Command = new RelayCommand(ShowAddEvidenceWindow)
                },
                
                // Изменение улики
                new PoliceDB.WPF.Models.MenuItem
                {
                    Title = "Изменить улику",
                    Description = "Внести изменения в существующую улику",
                    Icon = "✏️", // Карандаш
                    AllowedRoles = new[] { UserRole.SeniorInvestigator, UserRole.LawyerProsecutor, UserRole.Administrator },
                    Command = new RelayCommand(ShowModifyEvidenceWindow)
                },
                
                // Просмотр улик
                new PoliceDB.WPF.Models.MenuItem
                {
                    Title = "Просмотр улик",
                    Description = "Просмотр всех улик по делу",
                    Icon = "📋", // Список
                    AllowedRoles = new[] {
                        UserRole.Investigator,
                        UserRole.SeniorInvestigator,
                        UserRole.LawyerProsecutor,
                        UserRole.Judge,
                        UserRole.Administrator
                    },
                    Command = new RelayCommand(ShowViewEvidenceWindow)
                },
                
                // Описание дела
                new PoliceDB.WPF.Models.MenuItem
                {
                    Title = "Описание дела",
                    Description = "Просмотр и редактирование описания дела",
                    Icon = "📁", // Папка
                    AllowedRoles = new[] {
                        UserRole.Investigator,
                        UserRole.SeniorInvestigator,
                        UserRole.Juror,
                        UserRole.LawyerProsecutor,
                        UserRole.Judge,
                        UserRole.Administrator
                    },
                    Command = new RelayCommand(ShowCaseDetailsWindow)
                },
                
                // Вынесение приговора (ТОЛЬКО для судьи)
                new PoliceDB.WPF.Models.MenuItem
                {
                    Title = "Вынести приговор",
                    Description = "Вынесение окончательного приговора по делу",
                    Icon = "⚖️", // Весы правосудия
                    AllowedRoles = new[] { UserRole.Judge },
                    Command = new RelayCommand(ShowVerdictWindow)
                }
            };

            // Фильтруем пункты меню по роли пользователя
            var visibleItems = allMenuItems
                .Where(item => item.AllowedRoles.Contains(_currentUser.Role))
                .ToList();

            // Очищаем и добавляем только видимые пункты
            MenuItems.Clear();
            foreach (var item in visibleItems)
            {
                MenuItems.Add(item);
            }
        }

        // Методы для открытия окон
        private void ShowAddEvidenceWindow()
        {
            var addEvidenceWindow = new AddEvidenceWindow(_caseId, _currentUser);
            addEvidenceWindow.Owner = Application.Current.MainWindow;
            addEvidenceWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addEvidenceWindow.ShowDialog();
        }

        private void ShowModifyEvidenceWindow()
        {
            var modifyEvidenceWindow = new ModifyEvidenceWindow(_caseId, _currentUser);
            modifyEvidenceWindow.Owner = Application.Current.MainWindow;
            modifyEvidenceWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            modifyEvidenceWindow.ShowDialog();
        }

        private void ShowViewEvidenceWindow()
        {
            var evidenceListWindow = new EvidenceListWindow(_caseId, _currentUser);
            evidenceListWindow.Owner = Application.Current.MainWindow;
            evidenceListWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            evidenceListWindow.ShowDialog();
        }

        private void ShowCaseDetailsWindow()
        {
            var caseDetailsWindow = new CaseDetailsWindow(_caseId, _currentUser);
            caseDetailsWindow.Owner = Application.Current.MainWindow;
            caseDetailsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            caseDetailsWindow.ShowDialog();
        }

        private void ShowVerdictWindow()
        {
            var verdictWindow = new VerdictWindow(_caseId, _currentUser);
            verdictWindow.Owner = Application.Current.MainWindow;
            verdictWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            verdictWindow.ShowDialog();
        }
    }
}