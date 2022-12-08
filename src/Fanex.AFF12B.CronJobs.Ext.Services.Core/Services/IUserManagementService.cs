namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Services
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}
