using System.Windows.Input;

namespace PoliceDB.WPF.Models
{
    public class MenuItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public ICommand Command { get; set; } = null!;
        public bool IsVisible { get; set; } = true;

        // Для определения, кому доступен этот пункт
        public PoliceDB.Core.Models.UserRole[] AllowedRoles { get; set; } = Array.Empty<PoliceDB.Core.Models.UserRole>();
    }
}