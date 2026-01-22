using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PoliceDB.Core.Models;
using PoliceDB.WPF.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PoliceDB.WPF.ViewModels
{
    public partial class AddEvidenceViewModel : ObservableObject
    {
        private readonly string _caseId;
        private readonly User _currentUser;
        private readonly Window _window;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveEvidenceCommand))]
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
        private BitmapImage? _evidencePhoto;

        [ObservableProperty]
        private string _photoPath = string.Empty;

        [ObservableProperty]
        private string _photoFilePath = string.Empty;

        [ObservableProperty]
        private bool _isScrollBarVisible = false;

        // Команды
        public IRelayCommand AddParameterCommand { get; }
        public IRelayCommand RemoveParameterCommand { get; }
        public IRelayCommand AddPhotoCommand { get; }
        public IRelayCommand SaveEvidenceCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public Array EvidenceTypes => Enum.GetValues(typeof(EvidenceType));

        public AddEvidenceViewModel(Window window, string caseId, User currentUser)
        {
            _window = window;
            _caseId = caseId;
            _currentUser = currentUser;

            AddParameterCommand = new RelayCommand(AddParameter, CanAddParameter);
            RemoveParameterCommand = new RelayCommand<DynamicParameter>(RemoveParameter);
            AddPhotoCommand = new RelayCommand(AddPhoto);
            SaveEvidenceCommand = new RelayCommand(SaveEvidence, CanSaveEvidence);
            CancelCommand = new RelayCommand(Cancel);
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

                // Проверяем, нужно ли показывать скроллбар
                UpdateScrollBarVisibility();
            }
        }

        private void RemoveParameter(DynamicParameter parameter)
        {
            if (parameter != null)
            {
                Parameters.Remove(parameter);
                UpdateScrollBarVisibility();
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

                    // Просто показываем имя файла, без загрузки изображения
                    PhotoPath = System.IO.Path.GetFileName(PhotoFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка выбора файла: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    PhotoFilePath = string.Empty;
                    PhotoPath = string.Empty;
                }
            }
        }

        private bool CanSaveEvidence()
        {
            // Проверяем обязательные поля
            bool hasName = !string.IsNullOrWhiteSpace(EvidenceName);

            // Для отладки
            System.Diagnostics.Debug.WriteLine($"=== CanSaveEvidence ===");
            System.Diagnostics.Debug.WriteLine($"EvidenceName: '{EvidenceName}'");
            System.Diagnostics.Debug.WriteLine($"IsNullOrWhiteSpace: {string.IsNullOrWhiteSpace(EvidenceName)}");
            System.Diagnostics.Debug.WriteLine($"Returning: {hasName}");
            System.Diagnostics.Debug.WriteLine($"=== End CanSaveEvidence ===");

            return hasName;
        }

        private void SaveEvidence()
        {
            try
            {
                // Создаем объект улики
                var evidence = new PoliceDB.Core.Models.Evidence
                {
                    Id = Guid.NewGuid().ToString(),
                    Code = GenerateEvidenceCode(),
                    CaseId = _caseId,
                    Name = EvidenceName,
                    Type = SelectedEvidenceType,
                    Description = Description,
                    DiscoveryDate = DateTime.Now,
                    AddedByUserId = _currentUser.Id,
                    AddedDate = DateTime.UtcNow
                };

                // Добавляем фото как параметр
                if (!string.IsNullOrWhiteSpace(PhotoFilePath))
                {
                    evidence.Parameters["Фото"] = PhotoFilePath;
                }

                // Преобразуем параметры в Dictionary
                var parametersDict = new System.Collections.Generic.Dictionary<string, string>();
                foreach (var param in Parameters)
                {
                    if (!string.IsNullOrWhiteSpace(param.Name))
                    {
                        parametersDict[param.Name] = param.Value;
                    }
                }
                evidence.Parameters = parametersDict;

                // TODO: Здесь будет вызов сервиса для сохранения в базу данных
                MessageBox.Show($"Улика '{EvidenceName}' успешно добавлена!\nКод: {evidence.Code}",
                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                _window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении улики: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            // Проверяем, были ли внесены изменения
            bool hasChanges = !string.IsNullOrWhiteSpace(EvidenceName) ||
                             Parameters.Any() ||
                             !string.IsNullOrWhiteSpace(Description) ||
                             EvidencePhoto != null;

            if (hasChanges)
            {
                var result = MessageBox.Show("Все несохраненные изменения будут потеряны. Вы уверены?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            _window.Close();
        }

        private string GenerateEvidenceCode()
        {
            // Генерируем код вида CASE-001-EVD-001
            var random = new Random();
            return $"{_caseId}-EVD-{random.Next(1000, 9999)}";
        }

        private void UpdateScrollBarVisibility()
        {
            // Показываем скроллбар если больше 5 параметров
            IsScrollBarVisible = Parameters.Count > 5;
        }

        private string SavePhotoToStorage(string sourcePath, string evidenceCode)
        {
            if (string.IsNullOrEmpty(sourcePath)) return null;

            string targetDirectory = "C:\\PoliceDatabase\\EvidencePhotos\\";
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            string extension = System.IO.Path.GetExtension(sourcePath);
            string fileName = $"{evidenceCode}{extension}";
            string targetPath = System.IO.Path.Combine(targetDirectory, fileName);

            File.Copy(sourcePath, targetPath, true);
            return targetPath;
        }
    }
}