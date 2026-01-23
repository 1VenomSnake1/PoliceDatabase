using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using PoliceDB.DAL.Interfaces;
using PoliceDB.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PoliceDB.WPF.ViewModels
{
    public partial class ModifyEvidenceViewModel : ObservableObject
    {
        private readonly IEvidenceService _evidenceService;
        private readonly IPendingChangeRepository _pendingChangeRepository;
        private readonly string _caseId;
        private readonly User _currentUser;
        private readonly Window _window;

        // Режимы окна
        [ObservableProperty]
        private bool _isSearchMode = true;

        [ObservableProperty]
        private bool _isEditMode = false;

        // Поле для поиска
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchEvidenceCommand))]
        private string _evidenceCodeToSearch = string.Empty;

        // Данные для редактирования
        [ObservableProperty]
        private string _evidenceCode = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
        private string _evidenceName = string.Empty;

        [ObservableProperty]
        private EvidenceType _selectedEvidenceType = EvidenceType.Physical;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private ObservableCollection<DynamicParameter> _parameters = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddParameterCommand))]
        private string _newParameterName = string.Empty;

        [ObservableProperty]
        private string _newParameterValue = string.Empty;

        [ObservableProperty]
        private string _photoFilePath = string.Empty;

        [ObservableProperty]
        private string _photoPath = string.Empty;

        // ID оригинальной улики
        private string _originalEvidenceId = string.Empty;

        // Команды
        public IRelayCommand SearchEvidenceCommand { get; }
        public IRelayCommand AddParameterCommand { get; }
        public IRelayCommand RemoveParameterCommand { get; }
        public IRelayCommand AddPhotoCommand { get; }
        public IRelayCommand SaveChangesCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public Array EvidenceTypes => Enum.GetValues(typeof(EvidenceType));

        public ModifyEvidenceViewModel(Window window, string caseId, User currentUser,
            IEvidenceService evidenceService, IPendingChangeRepository pendingChangeRepository)
        {
            _window = window;
            _caseId = caseId;
            _currentUser = currentUser;
            _evidenceService = evidenceService;
            _pendingChangeRepository = pendingChangeRepository;

            // Инициализируем сервисы
            // В реальном приложении они должны внедряться через DI
            _evidenceService = new PoliceDB.BLL.Services.MockEvidenceService();
            _pendingChangeRepository = null; // Пока заглушка, нужно создать репозиторий для MongoDB

            SearchEvidenceCommand = new RelayCommand(SearchEvidence, CanSearchEvidence);
            AddParameterCommand = new RelayCommand(AddParameter, CanAddParameter);
            RemoveParameterCommand = new RelayCommand<DynamicParameter>(RemoveParameter);
            AddPhotoCommand = new RelayCommand(AddPhoto);
            SaveChangesCommand = new RelayCommand(SaveChanges, CanSaveChanges);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSearchEvidence()
        {
            return !string.IsNullOrWhiteSpace(EvidenceCodeToSearch);
        }

        private void SearchEvidence()
        {
            try
            {
                // Ищем улику по коду
                var evidence = _evidenceService.GetEvidenceByCode(EvidenceCodeToSearch);

                if (evidence == null)
                {
                    MessageBox.Show("Улика с таким кодом не найдена", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (evidence.CaseId != _caseId)
                {
                    MessageBox.Show("Улика не принадлежит данному делу", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохраняем ID оригинальной улики
                _originalEvidenceId = evidence.Id;

                // Загружаем данные для редактирования
                EvidenceCode = evidence.Code;
                EvidenceName = evidence.Name;
                SelectedEvidenceType = evidence.Type;
                Description = evidence.Description;

                // Загружаем параметры
                Parameters.Clear();
                if (evidence.Parameters != null)
                {
                    foreach (var param in evidence.Parameters)
                    {
                        if (param.Key != "Фото") // Фото обрабатываем отдельно
                        {
                            Parameters.Add(new DynamicParameter
                            {
                                Name = param.Key,
                                Value = param.Value
                            });
                        }
                        else
                        {
                            PhotoFilePath = param.Value;
                            PhotoPath = System.IO.Path.GetFileName(param.Value);
                        }
                    }
                }

                // Переключаемся в режим редактирования
                IsSearchMode = false;
                IsEditMode = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске улики: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddParameter()
        {
            return !string.IsNullOrWhiteSpace(NewParameterName);
        }

        private void AddParameter()
        {
            if (CanAddParameter())
            {
                Parameters.Add(new DynamicParameter
                {
                    Name = NewParameterName,
                    Value = NewParameterValue
                });
                NewParameterName = string.Empty;
                NewParameterValue = string.Empty;
            }
        }

        private void RemoveParameter(DynamicParameter parameter)
        {
            if (parameter != null)
            {
                Parameters.Remove(parameter);
            }
        }

        private void AddPhoto()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff",
                Title = "Выберите изображение улики"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    PhotoFilePath = openFileDialog.FileName;
                    PhotoPath = System.IO.Path.GetFileName(PhotoFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка выбора файла: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanSaveChanges()
        {
            return !string.IsNullOrWhiteSpace(EvidenceName);
        }

        private void SaveChanges()
        {
            try
            {
                // Создаем объект с изменениями
                var updatedEvidence = new PoliceDB.Core.Models.Evidence
                {
                    Id = _originalEvidenceId,
                    Code = EvidenceCode,
                    CaseId = _caseId,
                    Name = EvidenceName,
                    Type = SelectedEvidenceType,
                    Description = Description,
                    DiscoveryDate = DateTime.Now, // Можно оставить оригинальную дату или использовать текущую
                    AddedByUserId = _currentUser.Id,
                    AddedDate = DateTime.UtcNow
                };

                // Добавляем параметры
                var parametersDict = new System.Collections.Generic.Dictionary<string, string>();
                foreach (var param in Parameters)
                {
                    if (!string.IsNullOrWhiteSpace(param.Name))
                    {
                        parametersDict[param.Name] = param.Value;
                    }
                }

                // Добавляем фото, если есть
                if (!string.IsNullOrWhiteSpace(PhotoFilePath))
                {
                    parametersDict["Фото"] = PhotoFilePath;
                }

                updatedEvidence.Parameters = parametersDict;

                // Создаем запись об ожидающем изменении
                var pendingChange = new PendingChange
                {
                    ChangeType = ChangeType.EvidenceUpdate,
                    Status = ChangeStatus.Pending,
                    TargetId = _originalEvidenceId,
                    NewData = updatedEvidence,
                    RequestedByUserId = _currentUser.Id,
                    RequestedDate = DateTime.UtcNow,
                    Comment = "Изменение улики"
                };

                // TODO: Сохраняем в репозиторий PendingChange
                // _pendingChangeRepository.Add(pendingChange);

                MessageBox.Show("Изменения отправлены на рассмотрение администратору. " +
                    "Они будут применены после одобрения.", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                _window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            if (IsEditMode)
            {
                var result = MessageBox.Show("Все несохраненные изменения будут потеряны. Вы уверены?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            _window.Close();
        }
    }
}