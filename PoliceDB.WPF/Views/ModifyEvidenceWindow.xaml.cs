using Microsoft.Extensions.DependencyInjection;
using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class ModifyEvidenceWindow : Window
    {
        public ModifyEvidenceWindow(string caseId, User currentUser) // 2 параметра
        {
            InitializeComponent();

            // Получаем сервисы через App.ServiceProvider
            var evidenceService = App.ServiceProvider.GetRequiredService<PoliceDB.BLL.Interfaces.IEvidenceService>();
            var pendingChangeRepository = App.ServiceProvider.GetRequiredService<PoliceDB.DAL.Interfaces.IPendingChangeRepository>();

            DataContext = new ModifyEvidenceViewModel(this, caseId, currentUser, evidenceService, pendingChangeRepository);
        }
    }
}