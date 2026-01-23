using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class ModifyEvidenceWindow : Window
    {
        public ModifyEvidenceWindow(string caseId, User currentUser)
        {
            InitializeComponent();
            DataContext = new ModifyEvidenceViewModel(this, caseId, currentUser);
        }
    }
}