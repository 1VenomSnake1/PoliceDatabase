using PoliceDB.Core.Models;

namespace PoliceDB.BLL.Interfaces
{
    public interface IAdminService
    {
        List<PendingChange> GetPendingChanges();
        bool ApproveChange(string changeId, string adminUserId, string? comment = null);
        bool RejectChange(string changeId, string adminUserId, string? comment = null);
        List<User> GetAllUsers();
        User? CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(string userId);
    }
}