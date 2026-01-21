using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliceDB.Core.Models;
using PoliceDB.WPF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private ObservableCollection<MenuItem> _menuItems = new();

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
            var allMenuItems = new List<MenuItem>
            {
                // Добавление улики
                new MenuItem
                {
                    Title = "Добавить улику",
                    Description = "Добавить новую улику в дело",
                    Icon = "🔍", // Лупа
                    AllowedRoles = new[] { UserRole.Investigator, UserRole.Administrator },
                    Command = new RelayCommand(ShowAddEvidenceWindow)
                },
                
                // Изменение улики
                new MenuItem
                {
                    Title = "Изменить улику",
                    Description = "Внести изменения в существующую улику",
                    Icon = "✏️", // Карандаш
                    AllowedRoles = new[] { UserRole.SeniorInvestigator, UserRole.LawyerProsecutor, UserRole.Administrator },
                    Command = new RelayCommand(ShowModifyEvidenceWindow)
                },
                
                // Просмотр улик
                new MenuItem
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
                new MenuItem
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
                
                // Панель администратора (только для админа)
                new MenuItem
                {
                    Title = "Панель администратора",
                    Description = "Управление системой и подтверждение изменений",
                    Icon = "⚙️", // Шестеренка
                    AllowedRoles = new[] { UserRole.Administrator },
                    Command = new RelayCommand(ShowAdminPanelWindow)
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

        // Методы для открытия окон (пока заглушки)
        private void ShowAddEvidenceWindow()
        {
            MessageBox.Show("Открытие окна добавления улики", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowModifyEvidenceWindow()
        {
            MessageBox.Show("Открытие окна изменения улики", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowViewEvidenceWindow()
        {
            MessageBox.Show("Открытие окна просмотра улик", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowCaseDetailsWindow()
        {
            MessageBox.Show("Открытие окна описания дела", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAdminPanelWindow()
        {
            MessageBox.Show("Открытие панели администратора", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}