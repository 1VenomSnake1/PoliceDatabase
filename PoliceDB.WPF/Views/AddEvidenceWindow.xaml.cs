using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class AddEvidenceWindow : Window
    {
        public AddEvidenceWindow(string caseId, User currentUser)
        {
            InitializeComponent();
            DataContext = new AddEvidenceViewModel(this, caseId, currentUser);
        }
    }
}