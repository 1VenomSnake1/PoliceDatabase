using PoliceDB.Core.Models;

namespace PoliceDB.DAL.Interfaces
{
    public interface IEvidenceRepository
    {
        Evidence? GetById(string id);
        Evidence? GetByCode(string code);
        List<Evidence> GetByCaseId(string caseId);
        void Add(Evidence evidence);
        void Update(Evidence evidence);
        void Delete(string id);
        List<Evidence> GetAll();
        string GenerateEvidenceCode(string caseId);
    }
}