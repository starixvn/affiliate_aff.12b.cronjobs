using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.AFF12B.CronJobs.Core.Services
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
