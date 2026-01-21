using System;
using System.Collections.Generic;

namespace PoliceDB.Core.Models
{
    public enum EvidenceType
    {
        Physical,    // Вещественное
        Documentary, // Документальное
        Digital,     // Цифровое
        Biological,  // Биологическое
        Other        // Иное
    }

    public class Evidence
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Code { get; set; } = string.Empty; // Уникальный код улики
        public string CaseId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public EvidenceType Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public DateTime DiscoveryDate { get; set; }
        public string AddedByUserId { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}