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
                IsDescriptionInitialized = true,
                Status = CaseStatus.Trial, // Изменили на Trial для тестирования
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
                Description = "", // Пустое описание - можно создавать
                IsDescriptionInitialized = false,
                Status = CaseStatus.Open,
                CreatedDate = DateTime.UtcNow.AddDays(-15),
                CreatedByUserId = "1"
            },
            new Case
            {
                Id = "CASE-003",
                Title = "Дело об убийстве",
                Description = "Расследование убийства в центре города",
                IsDescriptionInitialized = true,
                Status = CaseStatus.ClosedNotGuilty, // Тестируем новый статус
                Verdict = "НЕ ВИНОВЕН",
                VerdictDate = DateTime.UtcNow.AddDays(-5),
                VerdictByUserId = "3", // ID судьи
                ClosedDate = DateTime.UtcNow.AddDays(-5),
                CreatedDate = DateTime.UtcNow.AddDays(-60),
                CreatedByUserId = "2"
            },
            new Case
            {
                Id = "CASE-004",
                Title = "Дело о взятке",
                Description = "Коррупционное преступление в правительстве",
                IsDescriptionInitialized = true,
                Status = CaseStatus.ClosedGuilty, // Тестируем другой новый статус
                Verdict = "ВИНОВЕН",
                VerdictDate = DateTime.UtcNow.AddDays(-3),
                VerdictByUserId = "3",
                ClosedDate = DateTime.UtcNow.AddDays(-3),
                CreatedDate = DateTime.UtcNow.AddDays(-45),
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
                IsDescriptionInitialized = !string.IsNullOrEmpty(description),
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
            @case.IsDescriptionInitialized = true;
            return true;
        }

        public bool UpdateCaseStatus(string caseId, CaseStatus status, string userId)
        {
            var @case = GetCase(caseId);
            if (@case == null) return false;

            // Если статус "закрыт с вердиктом", сохраняем дополнительную информацию
            if (status == CaseStatus.ClosedGuilty || status == CaseStatus.ClosedNotGuilty)
            {
                @case.Verdict = status == CaseStatus.ClosedGuilty ? "ВИНОВЕН" : "НЕ ВИНОВЕН";
                @case.VerdictDate = DateTime.UtcNow;
                @case.VerdictByUserId = userId;
                @case.ClosedDate = DateTime.UtcNow;
            }

            @case.Status = status;
            return true;
        }

        public bool UpdateVerdict(string caseId, string verdict, CaseStatus status, string userId)
        {
            var @case = GetCase(caseId);
            if (@case == null) return false;

            // Проверяем, что статус соответствует вердикту
            if ((status == CaseStatus.ClosedGuilty && verdict != "ВИНОВЕН") ||
                (status == CaseStatus.ClosedNotGuilty && verdict != "НЕ ВИНОВЕН"))
            {
                return false;
            }

            @case.Status = status;
            @case.Verdict = verdict;
            @case.VerdictDate = DateTime.UtcNow;
            @case.VerdictByUserId = userId;
            @case.ClosedDate = DateTime.UtcNow;

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

        public bool SubmitCaseDescriptionChange(string caseId, string description, string userId)
        {
            var @case = GetCase(caseId);
            if (@case == null) return false;

            // В мок-реализации просто обновляем описание
            return UpdateCaseDescription(caseId, description, userId);
        }
    }
}