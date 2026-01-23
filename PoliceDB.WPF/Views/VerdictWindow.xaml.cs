using Microsoft.Extensions.DependencyInjection;
using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class VerdictWindow : Window
    {
        public VerdictWindow(string caseId, User currentUser) // 2 параметра
        {
            InitializeComponent();

            // Получаем сервисы через App.ServiceProvider
            var caseService = App.ServiceProvider.GetRequiredService<PoliceDB.BLL.Interfaces.ICaseService>();

            DataContext = new VerdictViewModel(this, caseId, currentUser, caseService);
        }
    }
}