using Microsoft.Extensions.DependencyInjection;
using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class CaseDetailsWindow : Window
    {
        public CaseDetailsWindow(string caseId, User currentUser) // 2 параметра
        {
            InitializeComponent();

            // Получаем сервисы через App.ServiceProvider
            var caseService = App.ServiceProvider.GetRequiredService<PoliceDB.BLL.Interfaces.ICaseService>();

            DataContext = new CaseDetailsViewModel(this, caseId, currentUser, caseService);
        }
    }
}