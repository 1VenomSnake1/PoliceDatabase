using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceDB.BLL.Services
{
    public class MockEvidenceService : IEvidenceService
    {
        private readonly List<Evidence> _mockEvidences = new()
        {
            new Evidence
            {
                Id = "E1",
                Code = "CASE-001-EVD-001",
                CaseId = "CASE-001",
                Name = "Нож",
                Type = EvidenceType.Physical,
                Description = "Кухонный нож с отпечатками пальцев",
                DiscoveryDate = DateTime.UtcNow.AddDays(-25),
                AddedByUserId = "1",
                Parameters = new Dictionary<string, string>
                {
                    { "Длина", "25 см" },
                    { "Материал", "Сталь" },
                    { "Состояние", "Использован" }
                }
            },
            new Evidence
            {
                Id = "E2",
                Code = "CASE-001-EVD-002",
                CaseId = "CASE-001",
                Name = "Видеозапись с камеры",
                Type = EvidenceType.Digital,
                Description = "Запись с банковской камеры наблюдения",
                DiscoveryDate = DateTime.UtcNow.AddDays(-28),
                AddedByUserId = "1",
                Parameters = new Dictionary<string, string>
                {
                    { "Формат", "MP4" },
                    { "Длительность", "2 часа 15 минут" },
                    { "Качество", "HD" }
                }
            },
            new Evidence
            {
                Id = "E3",
                Code = "CASE-002-EVD-001",
                CaseId = "CASE-002",
                Name = "Договор купли-продажи",
                Type = EvidenceType.Documentary,
                Description = "Поддельный договор на недвижимость",
                DiscoveryDate = DateTime.UtcNow.AddDays(-10),
                AddedByUserId = "2",
                Parameters = new Dictionary<string, string>
                {
                    { "Количество страниц", "5" },
                    { "Тип документа", "Юридический" }
                }
            }
        };

        private int _evidenceCounter = 100;

        public Evidence? GetEvidence(string evidenceId)
        {
            return _mockEvidences.FirstOrDefault(e => e.Id == evidenceId);
        }

        public Evidence? GetEvidenceByCode(string code)
        {
            return _mockEvidences.FirstOrDefault(e => e.Code == code);
        }

        public List<Evidence> GetEvidencesByCase(string caseId)
        {
            return _mockEvidences.Where(e => e.CaseId == caseId).ToList();
        }

        public Evidence? AddEvidence(Evidence evidence, string userId)
        {
            evidence.Id = Guid.NewGuid().ToString();
            evidence.Code = $"{evidence.CaseId}-EVD-{_evidenceCounter++}";
            evidence.AddedByUserId = userId;
            evidence.AddedDate = DateTime.UtcNow;

            _mockEvidences.Add(evidence);
            return evidence;
        }

        public bool UpdateEvidence(Evidence evidence, string userId)
        {
            var existing = GetEvidence(evidence.Id);
            if (existing == null) return false;

            existing.Name = evidence.Name;
            existing.Type = evidence.Type;
            existing.Description = evidence.Description;
            existing.Parameters = evidence.Parameters;
            existing.DiscoveryDate = evidence.DiscoveryDate;

            return true;
        }

        public bool DeleteEvidence(string evidenceId, string userId)
        {
            var evidence = GetEvidence(evidenceId);
            if (evidence == null) return false;

            return _mockEvidences.Remove(evidence);
        }

        public bool SubmitEvidenceChangeRequest(Evidence evidence, string userId)
        {
            // В мок  версии просто обновляем
            return UpdateEvidence(evidence, userId);
        }
    }
}