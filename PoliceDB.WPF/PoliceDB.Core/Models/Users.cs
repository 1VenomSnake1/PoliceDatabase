using System;
using System.Collections.Generic;

namespace PoliceDB.Core.Models
{
    public enum UserRole
    {
        Investigator,        // Следователь
        SeniorInvestigator,  // Старший следователь
        Juror,              // Присяжный
        LawyerProsecutor,   // Адвокат/Прокурор
        Judge,              // Судья
        Administrator       // Администратор
    }

    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? DepartmentNumber { get; set; } // Для следователей
        public List<string> AllowedCaseIds { get; set; } = new();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}