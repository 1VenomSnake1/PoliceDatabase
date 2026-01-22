using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PoliceDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace PoliceDB.WPF.ViewModels
{
    public partial class EvidenceDetailsViewModel : ObservableObject
    {
        private readonly Evidence _evidence;
        private readonly Window _window;

        [ObservableProperty]
        private string _code = string.Empty;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _type = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private DateTime _discoveryDate;

        [ObservableProperty]
        private List<KeyValuePair<string, string>> _parameters = new();

        [ObservableProperty]
        private bool _hasPhoto;

        [ObservableProperty]
        private BitmapImage? _evidencePhoto;

        [ObservableProperty]
        private string _photoPath = string.Empty;

        [ObservableProperty]
        private DateTime _addedDate;

        public IRelayCommand BackCommand { get; }
        public IRelayCommand ViewPhotoCommand { get; }

        public EvidenceDetailsViewModel(Window window, Evidence evidence)
        {
            _window = window;
            _evidence = evidence;

            BackCommand = new RelayCommand(Back);
            ViewPhotoCommand = new RelayCommand(ViewPhoto);

            LoadEvidenceData();
        }

        private void LoadEvidenceData()
        {
            Code = _evidence.Code;
            Name = _evidence.Name;
            Type = _evidence.Type.ToString();
            Description = _evidence.Description;
            DiscoveryDate = _evidence.DiscoveryDate;
            AddedDate = _evidence.AddedDate;

            // Преобразуем параметры в список
            if (_evidence.Parameters != null)
            {
                Parameters = _evidence.Parameters
                    .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value))
                    .ToList();
            }

            // Проверяем наличие фото
            if (_evidence.Parameters != null && _evidence.Parameters.TryGetValue("Фото", out var photoPath))
            {
                PhotoPath = photoPath;
                HasPhoto = true;
            }
            else
            {
                HasPhoto = false;
            }
        }

        private void ViewPhoto()
        {
            if (HasPhoto && !string.IsNullOrEmpty(PhotoPath))
            {
                try
                {
                    // Открываем фото в стандартном просмотрщике
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = PhotoPath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть фото: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Back()
        {
            _window.Close();
        }
    }
}