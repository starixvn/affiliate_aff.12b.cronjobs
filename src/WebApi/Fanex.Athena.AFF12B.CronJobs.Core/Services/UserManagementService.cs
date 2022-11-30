using Fanex.Athena.AFF12B.CronJobs.Core.Extensions;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models.DTO;
using Microsoft.Extensions.Options;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly List<User> _users;

        public UserManagementService(IOptions<List<User>> userListAccessor)
        {
            _users = userListAccessor.Value;
        }

        public bool IsValidUser(string username, string password)
            => _users.Any(x => x.UserName.Equals(username)
            && x.Password.Equals(password.Sha256()));
    }
}
