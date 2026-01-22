using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class VerdictWindow : Window
    {
        public VerdictWindow(string caseId, User currentUser)
        {
            InitializeComponent();
            DataContext = new VerdictViewModel(this, caseId, currentUser);
        }
    }
}