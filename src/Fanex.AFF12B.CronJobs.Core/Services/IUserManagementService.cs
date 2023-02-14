namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}
