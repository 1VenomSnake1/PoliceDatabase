using PoliceDB.Core.Models;

namespace PoliceDB.BLL.Interfaces
{
    public interface IEvidenceService
    {
        Evidence? GetEvidence(string evidenceId);
        Evidence? GetEvidenceByCode(string code);
        List<Evidence> GetEvidencesByCase(string caseId);
        Evidence? AddEvidence(Evidence evidence, string userId);
        bool UpdateEvidence(Evidence evidence, string userId);
        bool DeleteEvidence(string evidenceId, string userId);
        bool SubmitEvidenceChangeRequest(Evidence evidence, string userId);
    }
}