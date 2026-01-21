using PoliceDB.Core.Models;

namespace PoliceDB.DAL.Interfaces
{
    public interface ICaseRepository
    {
        Case? GetById(string id);
        void Add(Case @case);
        void Update(Case @case);
        void Delete(string id);
        List<Case> GetAll();
        bool Exists(string id);
        List<Case> GetByUserId(string userId);
    }
}