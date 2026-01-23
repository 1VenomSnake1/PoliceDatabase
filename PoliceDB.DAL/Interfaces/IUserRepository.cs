using PoliceDB.Core.Models;

namespace PoliceDB.DAL.Interfaces
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        User? GetById(string id);
        void Add(User user);
        void Update(User user);
        void Delete(string id);
        List<User> GetAll();
        bool Exists(string username);
    }
}