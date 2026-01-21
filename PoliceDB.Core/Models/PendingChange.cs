using System;
using System.Collections.Generic;

namespace PoliceDB.Core.Models
{
    public enum ChangeType
    {
        EvidenceCreate,
        EvidenceUpdate,
        EvidenceDelete,
        CaseDescriptionUpdate
    }

    public enum ChangeStatus
    {
        Pending,    // Ожидает подтверждения
        Approved,   // Одобрено
        Rejected    // Отклонено
    }

    public class PendingChange
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ChangeType ChangeType { get; set; }
        public ChangeStatus Status { get; set; } = ChangeStatus.Pending;
        public string TargetId { get; set; } = string.Empty;
        public object NewData { get; set; } = new();
        public string RequestedByUserId { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
        public string? ApprovedByUserId { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? Comment { get; set; }
    }
}