using PoliceDB.Core.Models;

namespace PoliceDB.DAL.Interfaces
{
    public interface IPendingChangeRepository
    {
        PendingChange? GetById(string id);
        List<PendingChange> GetByStatus(ChangeStatus status);
        List<PendingChange> GetByUserId(string userId);
        void Add(PendingChange change);
        void Update(PendingChange change);
        void Delete(string id);
        List<PendingChange> GetAll();
    }
}