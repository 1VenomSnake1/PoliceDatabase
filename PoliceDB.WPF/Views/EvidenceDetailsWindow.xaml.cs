using PoliceDB.Core.Models;
using PoliceDB.WPF.ViewModels;
using System.Windows;

namespace PoliceDB.WPF.Views
{
    public partial class EvidenceDetailsWindow : Window
    {
        public EvidenceDetailsWindow(Evidence evidence, User currentUser)
        {
            InitializeComponent();
            DataContext = new EvidenceDetailsViewModel(this, evidence);
        }
    }
}