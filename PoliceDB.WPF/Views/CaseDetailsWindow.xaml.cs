using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class CaseDetailsWindow : Window
    {
        public CaseDetailsWindow(string caseId, User currentUser)
        {
            InitializeComponent();
            DataContext = new CaseDetailsViewModel(this, caseId, currentUser);
        }
    }
}