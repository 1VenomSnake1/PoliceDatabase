using PoliceDB.BLL.Interfaces;
using PoliceDB.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace PoliceDB.BLL.Services
{
    public class MockAuthService : IAuthService
    {
        private readonly List<User> _mockUsers = new()
        {
            new User
            {
                Id = "1",
                Username = "investigator",
                PasswordHash = "investigator123",
                Role = UserRole.Investigator,
                DepartmentNumber = "42",
                AllowedCaseIds = new List<string> { "CASE-001", "CASE-002" }
            },
            new User
            {
                Id = "2",
                Username = "senior",
                PasswordHash = "senior123",
                Role = UserRole.SeniorInvestigator,
                DepartmentNumber = "42",
                AllowedCaseIds = new List<string> { "CASE-001", "CASE-003" }
            },
            new User
            {
                Id = "3",
                Username = "judge",
                PasswordHash = "judge123",
                Role = UserRole.Judge,
                AllowedCaseIds = new List<string> { "CASE-001", "CASE-002", "CASE-003", "CASE-004" }
            },
            new User
            {
                Id = "4",
                Username = "admin",
                PasswordHash = "admin123",
                Role = UserRole.Administrator,
                AllowedCaseIds = new List<string> { "ALL" }
            },
            new User
            {
                Id = "5",
                Username = "lawyer",
                PasswordHash = "lawyer123",
                Role = UserRole.LawyerProsecutor,
                AllowedCaseIds = new List<string> { "CASE-001" }
            }
        };

        public User? Login(string username, string password, string caseId, UserRole role, string? departmentNumber)
        {
            // Сначала находим пользователя по логину
            var user = _mockUsers.FirstOrDefault(u => u.Username == username);

            if (user == null) return null;

            // Проверяем пароль
            if (user.PasswordHash != password) return null;

            // Проверяем роль - должно быть точное совпадение!
            if (user.Role != role) return null;

            // Проверка участка для следователей
            if ((role == UserRole.Investigator || role == UserRole.SeniorInvestigator)
                && user.DepartmentNumber != departmentNumber)
                return null;

            // Проверка доступа к делу
            if (!user.AllowedCaseIds.Contains("ALL") && !user.AllowedCaseIds.Contains(caseId))
                return null;

            return user;
        }

        public bool HasAccessToCase(string userId, string caseId)
        {
            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            return user.AllowedCaseIds.Contains("ALL") || user.AllowedCaseIds.Contains(caseId);
        }

        public bool CanModifyEvidence(string userId, string caseId)
        {
            if (!HasAccessToCase(userId, caseId)) return false;

            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            return user.Role switch
            {
                UserRole.Investigator => true,
                UserRole.SeniorInvestigator => true,
                UserRole.LawyerProsecutor => true,
                UserRole.Administrator => true,
                _ => false
            };
        }

        public bool CanViewEvidence(string userId, string caseId)
        {
            if (!HasAccessToCase(userId, caseId)) return false;

            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            if (user == null) return false;

            return user.Role switch
            {
                UserRole.Investigator => true,
                UserRole.SeniorInvestigator => true,
                UserRole.LawyerProsecutor => true,
                UserRole.Judge => true,
                UserRole.Administrator => true,
                _ => false
            };
        }

        public bool CanChangeCaseStatus(string userId)
        {
            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            return user?.Role == UserRole.Judge || user?.Role == UserRole.Administrator;
        }

        public bool CanApproveChanges(string userId)
        {
            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            return user?.Role == UserRole.Administrator;
        }

        public bool CanCreateCaseDescription(string userId, string caseId)
        {
            if (!HasAccessToCase(userId, caseId)) return false;

            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            return user?.Role == UserRole.Investigator;
        }

        public bool CanViewCaseDescription(string userId)
        {
            var user = _mockUsers.FirstOrDefault(u => u.Id == userId);
            return user != null; // Все авторизованные пользователи
        }
    }
}