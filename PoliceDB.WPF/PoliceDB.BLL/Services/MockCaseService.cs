using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoliceDB.BLL.Services
{
    public class MockCaseService : ICaseService
    {
        private readonly List<Case> _mockCases = new()
        {
            new Case
            {
                Id = "CASE-001",
                Title = "Дело о краже в банке",
                Description = "Расследование кражи денежных средств из банковского хранилища",
                Status = CaseStatus.Investigation,
                CreatedDate = DateTime.UtcNow.AddDays(-30),
                CreatedByUserId = "1",
                Protocols = new List<CourtProtocol>
                {
                    new CourtProtocol
                    {
                        Id = "P1",
                        Date = DateTime.UtcNow.AddDays(-10),
                        Content = "Первое судебное заседание. Заслушаны показания свидетелей."
                    }
                }
            },
            new Case
            {
                Id = "CASE-002",
                Title = "Дело о мошенничестве",
                Description = "Крупное мошенничество с недвижимостью",
                Status = CaseStatus.Open,
                CreatedDate = DateTime.UtcNow.AddDays(-15),
                CreatedByUserId = "1"
            },
            new Case
            {
                Id = "CASE-003",
                Title = "Дело об убийстве",
                Description = "Расследование убийства в центре города",
                Status = CaseStatus.Closed,
                CreatedDate = DateTime.UtcNow.AddDays(-60),
                CreatedByUserId = "2"
            }
        };

        public Case? GetCase(string caseId)
        {
            return _mockCases.FirstOrDefault(c => c.Id == caseId);
        }

        public Case? CreateCase(string caseId, string title, string description, string userId)
        {
            var newCase = new Case
            {
                Id = caseId,
                Title = title,
                Description = description,
                Status = CaseStatus.Open,
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            _mockCases.Add(newCase);
            return newCase;
        }

        public bool UpdateCaseDescription(string caseId, string description, string userId)
        {
            var @case = GetCase(caseId);
            if (@case == null) return false;

            @case.Description = description;
            return true;
        }

        public bool UpdateCaseStatus(string caseId, CaseStatus status, string userId)
        {
            var @case = GetCase(caseId);
            if (@case == null) return false;

            @case.Status = status;
            return true;
        }

        public bool AddProtocol(string caseId, CourtProtocol protocol, string userId)
        {
            var @case = GetCase(caseId);
            if (@case == null) return false;

            protocol.Id = Guid.NewGuid().ToString();
            @case.Protocols.Add(protocol);
            return true;
        }

        public List<Case> GetUserCases(string userId)
        {
            // В реальном приложении здесь была бы проверка прав доступа
            return _mockCases;
        }
    }
}