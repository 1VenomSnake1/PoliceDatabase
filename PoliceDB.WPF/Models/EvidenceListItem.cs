using System;
using System.Windows.Input;

namespace PoliceDB.WPF.Models
{
    public class EvidenceListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime DiscoveryDate { get; set; }
        public ICommand ViewDetailsCommand { get; set; } = null!;
    }
}