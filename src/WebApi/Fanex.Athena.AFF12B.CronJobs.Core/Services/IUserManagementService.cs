namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Services
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}
