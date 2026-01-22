using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using PoliceDB.WPF.Models;
using PoliceDB.WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PoliceDB.WPF.ViewModels
{
    public partial class EvidenceListViewModel : ObservableObject
    {
        private readonly IEvidenceService _evidenceService;
        private readonly string _caseId;
        private readonly User _currentUser;
        private readonly Window _window;

        [ObservableProperty]
        private ObservableCollection<EvidenceListItem> _evidences = new();

        [ObservableProperty]
        private bool _hasEvidences;

        [ObservableProperty]
        private bool _hasParameters;

        [ObservableProperty]
        private bool _noEvidences;

        public IRelayCommand BackCommand { get; }
        public IRelayCommand RefreshCommand { get; }

        public EvidenceListViewModel(Window window, string caseId, User currentUser)
        {
            _window = window;
            _caseId = caseId;
            _currentUser = currentUser;

            // Используем MockEvidenceService для демонстрации
            // В реальном приложении нужно внедрять через DI
            _evidenceService = new PoliceDB.BLL.Services.MockEvidenceService();

            BackCommand = new RelayCommand(Back);
            RefreshCommand = new RelayCommand(RefreshEvidences);

            LoadEvidences();
        }

        private void LoadEvidences()
        {
            try
            {
                Evidences.Clear();

                // Получаем улики по делу
                var evidences = _evidenceService.GetEvidencesByCase(_caseId);

                foreach (var evidence in evidences)
                {
                    Evidences.Add(new EvidenceListItem
                    {
                        Id = evidence.Id,
                        Code = evidence.Code,
                        Name = evidence.Name,
                        Type = evidence.Type.ToString(),
                        DiscoveryDate = evidence.DiscoveryDate,
                        ViewDetailsCommand = new RelayCommand<string>(ViewEvidenceDetails)
                    });
                }

                HasEvidences = Evidences.Any();
                NoEvidences = !HasEvidences;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки улик: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewEvidenceDetails(string? evidenceId)
        {
            if (string.IsNullOrEmpty(evidenceId)) return;

            try
            {
                var evidence = _evidenceService.GetEvidence(evidenceId);
                if (evidence != null)
                {
                    var detailsWindow = new EvidenceDetailsWindow(evidence, _currentUser);
                    detailsWindow.Owner = _window;
                    detailsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    detailsWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия деталей улики: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshEvidences()
        {
            LoadEvidences();
        }

        private void Back()
        {
            _window.Close();
        }
    }
}