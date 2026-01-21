using PoliceDB.Core.Models;

namespace PoliceDB.BLL.Interfaces
{
    public interface IAuthService
    {
        User? Login(string username, string password, string caseId, UserRole role, string? departmentNumber);
        bool HasAccessToCase(string userId, string caseId);
        bool CanModifyEvidence(string userId, string caseId);
        bool CanViewEvidence(string userId, string caseId);
        bool CanChangeCaseStatus(string userId);
        bool CanApproveChanges(string userId);
        bool CanCreateCaseDescription(string userId, string caseId);
        bool CanViewCaseDescription(string userId);
    }
}