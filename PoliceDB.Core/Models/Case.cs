using System;
using System.Collections.Generic;

namespace PoliceDB.Core.Models
{
    public enum CaseStatus
    {
        Open,           // Открыто (зеленый)
        Investigation,  // Расследование (желтый)
        Trial,         // Судебное разбирательство (желтый)
        Closed,        // Закрыто (красный)
        ClosedGuilty,   // Закрыто: ВИНОВЕН (красный)
        ClosedNotGuilty // Закрыто: НЕ ВИНОВЕН (зеленый)
    }

    public class Case
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CaseStatus Status { get; set; } = CaseStatus.Open;
        public string? Verdict { get; set; } // Текст вердикта
        public DateTime? VerdictDate { get; set; } // Дата вынесения вердикта
        public string? VerdictByUserId { get; set; } // Кто вынес вердикт
        public bool IsDescriptionInitialized { get; set; } = false;
        public List<CourtProtocol> Protocols { get; set; } = new();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime? ClosedDate { get; set; } // Дата закрытия дела
    }

    public class CourtProtocol
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Date { get; set; }
        public string Content { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}