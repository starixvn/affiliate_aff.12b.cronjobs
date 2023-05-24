using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Generic;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.UnitTest.Services
{
    public class UserManagementServiceTest
    {
        private readonly List<User> users = new List<User> { new User { UserName = "test", Password = "1234".Sha256() } };
        private readonly IOptions<List<User>> _mockUsers;

        public UserManagementServiceTest()
        {
            _mockUsers = Substitute.For<IOptions<List<User>>>();
        }

        [Fact]
        public void IsValidUser_WhenCalled_GenerateHasedPassword()
        {
            //Arrange
            var rawUser = new User
            {
                UserName = "UserName",
                Password = "Password"
            };
            var hashedPassword = rawUser.Password.Sha256();
            _mockUsers.Value.Returns(new List<User> { new User { UserName = rawUser.UserName, Password = hashedPassword } });

            var userManagementService = new UserManagementService(_mockUsers);

            //Action
            var result = userManagementService.IsValidUser(rawUser.UserName, rawUser.Password);

            //Assert
            Assert.True(result);
        }


        [Theory]
        [InlineData("test", "123", false)]
        [InlineData(null, null, false)]
        [InlineData(null, "1234", false)]
        [InlineData("test", null, false)]
        [InlineData("test", "1234", true)]
        public void IsValidUser_WhenCalled_ReturnCorrectResult(string userName, string pass, bool expectedResult)
        {
            //Arrange
            _mockUsers.Value.Returns(users);
            var userManagementService = new UserManagementService(_mockUsers);

            //Action
            var result = userManagementService.IsValidUser(userName, pass);

            //Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
