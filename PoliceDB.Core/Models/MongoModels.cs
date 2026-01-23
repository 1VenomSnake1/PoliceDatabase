using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace PoliceDB.Core.Models
{
    // Базовый класс для всех документов MongoDB
    public abstract class MongoDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // Модель пользователя для MongoDB
    [BsonIgnoreExtraElements]
    public class MongoUser : MongoDocument
    {
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty; // "Investigator", "SeniorInvestigator", etc.

        [BsonElement("departmentNumber")]
        public string? DepartmentNumber { get; set; }

        [BsonElement("allowedCaseIds")]
        public List<string> AllowedCaseIds { get; set; } = new();

        [BsonElement("fullName")]
        public string? FullName { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }

    // Модель дела для MongoDB
    [BsonIgnoreExtraElements]
    public class MongoCase : MongoDocument
    {
        [BsonElement("caseId")]
        public string CaseId { get; set; } = string.Empty; // Уникальный номер дела

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("status")]
        public string Status { get; set; } = "Open"; // "Open", "Investigation", "Trial", "Closed", "ClosedGuilty", "ClosedNotGuilty"

        [BsonElement("verdict")]
        public string? Verdict { get; set; }

        [BsonElement("verdictDate")]
        public DateTime? VerdictDate { get; set; }

        [BsonElement("verdictByUserId")]
        public string? VerdictByUserId { get; set; }

        [BsonElement("isDescriptionInitialized")]
        public bool IsDescriptionInitialized { get; set; } = false;

        [BsonElement("protocols")]
        public List<MongoCourtProtocol> Protocols { get; set; } = new();

        [BsonElement("createdByUserId")]
        public string CreatedByUserId { get; set; } = string.Empty;

        [BsonElement("closedDate")]
        public DateTime? ClosedDate { get; set; }

        [BsonElement("assignedUsers")]
        public List<string> AssignedUsers { get; set; } = new(); // ID пользователей, работающих с делом

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new(); // Теги для поиска
    }

    // Модель протокола судебного заседания
    [BsonIgnoreExtraElements]
    public class MongoCourtProtocol
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("filePath")]
        public string? FilePath { get; set; }

        [BsonElement("createdByUserId")]
        public string CreatedByUserId { get; set; } = string.Empty;
    }

    // Модель улики для MongoDB
    [BsonIgnoreExtraElements]
    public class MongoEvidence : MongoDocument
    {
        [BsonElement("code")]
        public string Code { get; set; } = string.Empty; // Уникальный код улики

        [BsonElement("caseId")]
        public string CaseId { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("type")]
        public string Type { get; set; } = "Physical"; // "Physical", "Documentary", "Digital", "Biological", "Other"

        [BsonElement("parameters")]
        public Dictionary<string, string> Parameters { get; set; } = new();

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("discoveryDate")]
        public DateTime DiscoveryDate { get; set; }

        [BsonElement("addedByUserId")]
        public string AddedByUserId { get; set; } = string.Empty;

        [BsonElement("photoPath")]
        public string? PhotoPath { get; set; }

        [BsonElement("storageLocation")]
        public string? StorageLocation { get; set; } // Место хранения физической улики

        [BsonElement("status")]
        public string Status { get; set; } = "Active"; // "Active", "Archived", "Destroyed"
    }

    // Модель ожидающих изменений для MongoDB
    [BsonIgnoreExtraElements]
    public class MongoPendingChange : MongoDocument
    {
        [BsonElement("changeType")]
        public string ChangeType { get; set; } = string.Empty; // "EvidenceUpdate", "EvidenceDelete", "CaseDescriptionUpdate"

        [BsonElement("status")]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

        [BsonElement("targetId")]
        public string TargetId { get; set; } = string.Empty; // ID изменяемого объекта

        [BsonElement("targetType")]
        public string TargetType { get; set; } = string.Empty; // "Evidence", "Case"

        [BsonElement("newData")]
        public BsonDocument NewData { get; set; } = new BsonDocument(); // Новые данные в формате BSON

        [BsonElement("requestedByUserId")]
        public string RequestedByUserId { get; set; } = string.Empty;

        [BsonElement("approvedByUserId")]
        public string? ApprovedByUserId { get; set; }

        [BsonElement("approvedDate")]
        public DateTime? ApprovedDate { get; set; }

        [BsonElement("comment")]
        public string? Comment { get; set; }

        [BsonElement("priority")]
        public int Priority { get; set; } = 1; // 1 - низкий, 2 - средний, 3 - высокий
    }
}