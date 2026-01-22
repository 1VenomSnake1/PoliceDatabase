using PoliceDB.Core.Models;

namespace PoliceDB.BLL.Interfaces
{
    public interface ICaseService
    {
        Case? GetCase(string caseId);
        Case? CreateCase(string caseId, string title, string description, string userId);
        bool UpdateCaseDescription(string caseId, string description, string userId);
        bool UpdateCaseStatus(string caseId, CaseStatus status, string userId);
        bool UpdateVerdict(string caseId, string verdict, CaseStatus status, string userId);
        bool AddProtocol(string caseId, CourtProtocol protocol, string userId);
        List<Case> GetUserCases(string userId);
        bool SubmitCaseDescriptionChange(string caseId, string description, string userId);
    }
}