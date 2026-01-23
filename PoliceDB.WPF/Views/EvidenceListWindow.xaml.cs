using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class EvidenceListWindow : Window
    {
        public EvidenceListWindow(string caseId, User currentUser)
        {
            InitializeComponent();
            DataContext = new EvidenceListViewModel(this, caseId, currentUser);
        }
    }
}