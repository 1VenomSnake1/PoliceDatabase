using MongoDB.Bson;
using PoliceDB.Core.Models;

namespace PoliceDB.DAL.Mappers
{
    public static class MongoMapper
    {
        // Преобразование User -> MongoUser
        public static MongoUser ToMongoUser(this User user)
        {
            return new MongoUser
            {
                Id = user.Id,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                Role = user.Role.ToString(),
                DepartmentNumber = user.DepartmentNumber,
                AllowedCaseIds = user.AllowedCaseIds,
                FullName = user.Username, // Временно, если нужно
                Email = $"{user.Username}@police.gov", // Временно
                IsActive = true,
                CreatedAt = user.CreatedDate,
                UpdatedAt = user.CreatedDate
            };
        }

        // Преобразование MongoUser -> User
        public static User ToDomainUser(this MongoUser mongoUser)
        {
            if (!Enum.TryParse<UserRole>(mongoUser.Role, out var role))
            {
                role = UserRole.Investigator;
            }

            return new User
            {
                Id = mongoUser.Id,
                Username = mongoUser.Username ?? string.Empty, // Важно: проверка на null
                PasswordHash = mongoUser.PasswordHash ?? string.Empty,
                Role = role,
                DepartmentNumber = mongoUser.DepartmentNumber,
                AllowedCaseIds = mongoUser.AllowedCaseIds ?? new List<string>(),
                CreatedDate = mongoUser.CreatedAt
            };
        }

        // Преобразование Case -> MongoCase
        public static MongoCase ToMongoCase(this Case domainCase)
        {
            return new MongoCase
            {
                Id = domainCase.Id,
                CaseId = domainCase.Id,
                Title = domainCase.Title,
                Description = domainCase.Description,
                Status = domainCase.Status.ToString(),
                Verdict = domainCase.Verdict,
                VerdictDate = domainCase.VerdictDate,
                VerdictByUserId = domainCase.VerdictByUserId,
                IsDescriptionInitialized = domainCase.IsDescriptionInitialized,
                CreatedByUserId = domainCase.CreatedByUserId,
                ClosedDate = domainCase.ClosedDate,
                CreatedAt = domainCase.CreatedDate,
                UpdatedAt = domainCase.CreatedDate,
                Protocols = domainCase.Protocols?.Select(p => new MongoCourtProtocol
                {
                    Date = p.Date,
                    Content = p.Content,
                    FilePath = p.FilePath,
                    CreatedByUserId = domainCase.CreatedByUserId
                }).ToList() ?? new List<MongoCourtProtocol>()
            };
        }

        // Преобразование MongoCase -> Case
        public static Case ToDomainCase(this MongoCase mongoCase)
        {
            if (!Enum.TryParse<CaseStatus>(mongoCase.Status, out var status))
            {
                status = CaseStatus.Open;
            }

            return new Case
            {
                Id = mongoCase.CaseId,
                Title = mongoCase.Title,
                Description = mongoCase.Description,
                Status = status,
                Verdict = mongoCase.Verdict,
                VerdictDate = mongoCase.VerdictDate,
                VerdictByUserId = mongoCase.VerdictByUserId,
                IsDescriptionInitialized = mongoCase.IsDescriptionInitialized,
                CreatedByUserId = mongoCase.CreatedByUserId,
                ClosedDate = mongoCase.ClosedDate,
                CreatedDate = mongoCase.CreatedAt,
                Protocols = mongoCase.Protocols?.Select(p => new CourtProtocol
                {
                    Id = p.Id,
                    Date = p.Date,
                    Content = p.Content,
                    FilePath = p.FilePath
                }).ToList() ?? new List<CourtProtocol>()
            };
        }

        // Преобразование Evidence -> MongoEvidence
        public static MongoEvidence ToMongoEvidence(this Evidence evidence)
        {
            return new MongoEvidence
            {
                Id = evidence.Id,
                Code = evidence.Code,
                CaseId = evidence.CaseId,
                Name = evidence.Name,
                Type = evidence.Type.ToString(),
                Parameters = evidence.Parameters ?? new Dictionary<string, string>(),
                Description = evidence.Description,
                DiscoveryDate = evidence.DiscoveryDate,
                AddedByUserId = evidence.AddedByUserId,
                PhotoPath = evidence.PhotoPath,
                CreatedAt = evidence.AddedDate,
                UpdatedAt = evidence.AddedDate
            };
        }

        // Преобразование MongoEvidence -> Evidence
        public static Evidence ToDomainEvidence(this MongoEvidence mongoEvidence)
        {
            if (!Enum.TryParse<EvidenceType>(mongoEvidence.Type, out var evidenceType))
            {
                evidenceType = EvidenceType.Physical;
            }

            return new Evidence
            {
                Id = mongoEvidence.Id,
                Code = mongoEvidence.Code,
                CaseId = mongoEvidence.CaseId,
                Name = mongoEvidence.Name,
                Type = evidenceType,
                Parameters = mongoEvidence.Parameters ?? new Dictionary<string, string>(),
                Description = mongoEvidence.Description,
                DiscoveryDate = mongoEvidence.DiscoveryDate,
                AddedByUserId = mongoEvidence.AddedByUserId,
                PhotoPath = mongoEvidence.PhotoPath,
                AddedDate = mongoEvidence.CreatedAt
            };
        }

        // Преобразование PendingChange -> MongoPendingChange
        public static MongoPendingChange ToMongoPendingChange(this PendingChange pendingChange)
        {
            return new MongoPendingChange
            {
                Id = pendingChange.Id,
                ChangeType = pendingChange.ChangeType.ToString(),
                Status = pendingChange.Status.ToString(),
                TargetId = pendingChange.TargetId,
                TargetType = GetTargetType(pendingChange.ChangeType),
                NewData = MongoDB.Bson.BsonDocument.Parse(
                    System.Text.Json.JsonSerializer.Serialize(pendingChange.NewData)
                ),
                RequestedByUserId = pendingChange.RequestedByUserId,
                ApprovedByUserId = pendingChange.ApprovedByUserId,
                ApprovedDate = pendingChange.ApprovedDate,
                Comment = pendingChange.Comment,
                CreatedAt = pendingChange.RequestedDate,
                UpdatedAt = pendingChange.RequestedDate
            };
        }

        // Преобразование MongoPendingChange -> PendingChange
        public static PendingChange ToDomainPendingChange(this MongoPendingChange mongoPendingChange)
        {
            if (!Enum.TryParse<ChangeType>(mongoPendingChange.ChangeType, out var changeType))
            {
                changeType = ChangeType.EvidenceUpdate;
            }

            if (!Enum.TryParse<ChangeStatus>(mongoPendingChange.Status, out var status))
            {
                status = ChangeStatus.Pending;
            }

            // Десериализуем NewData в зависимости от типа
            object newData = DeserializeNewData(mongoPendingChange.NewData, changeType);

            return new PendingChange
            {
                Id = mongoPendingChange.Id,
                ChangeType = changeType,
                Status = status,
                TargetId = mongoPendingChange.TargetId,
                NewData = newData,
                RequestedByUserId = mongoPendingChange.RequestedByUserId,
                ApprovedByUserId = mongoPendingChange.ApprovedByUserId,
                ApprovedDate = mongoPendingChange.ApprovedDate,
                Comment = mongoPendingChange.Comment,
                RequestedDate = mongoPendingChange.CreatedAt
            };
        }

        private static string GetTargetType(ChangeType changeType)
        {
            return changeType switch
            {
                ChangeType.EvidenceCreate or ChangeType.EvidenceUpdate or ChangeType.EvidenceDelete => "Evidence",
                ChangeType.CaseDescriptionUpdate => "Case",
                _ => "Unknown"
            };
        }

        private static object DeserializeNewData(MongoDB.Bson.BsonDocument newData, ChangeType changeType)
        {
            try
            {
                string json = newData.ToJson();

                return changeType switch
                {
                    ChangeType.EvidenceCreate or ChangeType.EvidenceUpdate or ChangeType.EvidenceDelete =>
                        System.Text.Json.JsonSerializer.Deserialize<Evidence>(json) ?? new Evidence(),

                    ChangeType.CaseDescriptionUpdate =>
                        System.Text.Json.JsonSerializer.Deserialize<string>(json) ?? string.Empty,

                    _ => new object()
                };
            }
            catch
            {
                return changeType switch
                {
                    ChangeType.CaseDescriptionUpdate => string.Empty,
                    _ => new Evidence()
                };
            }
        }
    }
}