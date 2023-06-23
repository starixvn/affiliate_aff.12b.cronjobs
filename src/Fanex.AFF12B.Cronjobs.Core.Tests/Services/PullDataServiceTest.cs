using Fanex.AFF12B.CronJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Services
{
    public class PullDataServiceTest
    {
        private readonly IThirdPartyProductApi _thirdPartyProductApi;

        public PullDataServiceTest()
        {
            _thirdPartyProductApi = Substitute.For<IThirdPartyProductApi>();
        }

        [Fact]
        public async Task GetJsonDataListAsync_WhenCallWithInvalidRangeTime_ReturnEmptyAndNotCallApi()
        {
            //Arrange
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 00, 00, 00),
                MaxMinutesRange = 30
            };
            var service = CreatePullDataService();

            //Action
            var result = await service.GetJsonDataListAsync(param);

            //Assert
            Assert.Empty(result);
            await _thirdPartyProductApi.DidNotReceive().GetTPGBetTrans(Arg.Any<ThirdPartyApiParam>());
        }

        [Fact]
        public async Task GetJsonDataListAsync_WhenCallWithApiReturnEmpty_ThrowException()
        {
            //Arrange
            _thirdPartyProductApi.GetTPGBetTrans(Arg.Any<ThirdPartyApiParam>()).Returns(Task.FromResult<ThirdPartyProductRaw>(null));
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 01, 00, 00),
                TpType = 1,
                MaxMinutesRange = 30
            };
            var service = CreatePullDataService();

            //Action
            var ex = await Record.ExceptionAsync(() => service.GetJsonDataListAsync(param));

            //Assert
            Assert.NotNull(ex);
            Assert.Contains("12Emerald - Playtech Job(TpType: '1', SubItem: '') - Nothing respond from Api", ex.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("GT_F_001")]
        public async Task GetJsonDataListAsync_WhenCallWithApiReturnErrorCode_ThrowException(string errorCode)
        {
            //Arrange
            var rawData = new ThirdPartyProductRaw
            {
                SystemCode = errorCode
            };

            _thirdPartyProductApi.GetTPGBetTrans(Arg.Any<ThirdPartyApiParam>()).Returns(rawData);
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 01, 00, 00),
                TpType = 1,
                MaxMinutesRange = 30
            };
            var service = CreatePullDataService();

            //Action
            var ex = await Record.ExceptionAsync(() => service.GetJsonDataListAsync(param));

            //Assert
            Assert.NotNull(ex);
            Assert.Contains($"12Emerald - Playtech Job(TpType: '1', SubItem: '') - The 3rd api return ErrorCode: {errorCode}", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("[]")]
        [InlineData("[]  ")]
        public async Task GetJsonDataListAsync_WhenCallWithApiReturnEmptyTicket_ReturnCorrectData(string betTransTicket)
        {
            //Arrange
            var rawData = new ThirdPartyProductRaw
            {
                BetTransTicket = betTransTicket,
                VersionKey = null,
                SystemCode = "GT_S_001",
                SystemMessage = "Get Ticket Success But No Ticket"
            };

            _thirdPartyProductApi.GetTPGBetTrans(Arg.Any<ThirdPartyApiParam>()).Returns(rawData);
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 01, 00, 00),
                TpType = 1,
                MaxMinutesRange = 30
            };
            var service = CreatePullDataService();

            //Action
            var result = await service.GetJsonDataListAsync(param);
            var item = result.FirstOrDefault();

            //Assert
            Assert.Empty(item.BetTransTicket);
        }

        [Fact]
        public async Task GetJsonDataListAsync_WhenCallWithValidParam_ReturnCorrectData()
        {
            //Arrange
            _thirdPartyProductApi.GetTPGBetTrans(Arg.Any<ThirdPartyApiParam>()).Returns(RawData);
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 01, 00, 00),
                MaxMinutesRange = 30
            };
            var service = CreatePullDataService();

            //Action
            var result = await service.GetJsonDataListAsync(param);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetJsonDataListAsync_WhenCallWithValidParam_BetTicketMoreMaxRange_ReturnCorrectData()
        {
            //Arrange
            var rawData = new ThirdPartyProductRaw
            {
                BetTransTicket = GetBetTransTicketList(),
                Count = 2100,
                VersionKey = null,
                SystemCode = "GT_S_000",
                SystemMessage = "Get Ticket Success"
            };

            _thirdPartyProductApi.GetTPGBetTrans(Arg.Any<ThirdPartyApiParam>()).Returns(rawData);
            var param = new ThirdPartyApiParam
            {
                BeginDate = new DateTime(2023, 06, 10, 00, 00, 00),
                EndDate = new DateTime(2023, 06, 10, 00, 30, 00),
                MaxMinutesRange = 30
            };
            var service = CreatePullDataService();

            //Action
            var result = await service.GetJsonDataListAsync(param);
            var firstItem = result.FirstOrDefault();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(2000, firstItem.Count);
        }

        public PullDataService CreatePullDataService() => new PullDataService(_thirdPartyProductApi);

        private static ThirdPartyProductRaw RawData => new ThirdPartyProductRaw
        {
            BetTransTicket = "[{\"bet\":100.00,\"betstatus\":\"Win\",\"datainfo\":\"{\\\"BundleId\\\":\\\"3181547310\\\",\\\"BetId\\\":\\\"6484944087\\\",\\\"TableId\\\":\\\"1\\\",\\\"Status\\\":\\\"1\\\",\\\"UserId\\\":\\\"Bi1234zz\\\",\\\"GameType\\\":\\\"C baccarat\\\",\\\"Bet\\\":100.00,\\\"WinLose\\\":100.00,\\\"Balance\\\":492.85,\\\"Currency\\\":\\\"VND\\\",\\\"GameId\\\":\\\"1\\\",\\\"GameCode\\\":\\\"19-44\\\",\\\"Platform\\\":\\\"h5_mobile\\\",\\\"RoundId\\\":\\\"149892356\\\",\\\"PlayerHand\\\":\\\"Player\\\",\\\"GameResult\\\":\\\"C6H8S4#H3D4DQ\\\",\\\"TransDate\\\":\\\"2023-04-30 00:00:05\\\",\\\"IsHedgeBet\\\":false}\",\"gameplatform\":\"gpilive\",\"ishedgebet\":false,\"thirdpartyid\":7,\"transactiondatetime\":\"2023-04-30T00:00:05\",\"transactionid\":\"6484944087\",\"username\":\"Bi1234zz\",\"validbet\":100.00,\"win\":200.00}]",
            Count = 1,
            VersionKey = null,
            SystemCode = "GT_S_000",
            SystemMessage = "Get Ticket Success"
        };

        private static string GetBetTransTicketList()
        {
            var betTransTicket = new List<Ticket>
            {
                new Ticket()
            };

            while (betTransTicket.Count() < 2100)
            {
                betTransTicket.Add(new Ticket());
            }

            return JsonConvert.SerializeObject(betTransTicket);
        }
    }
}
