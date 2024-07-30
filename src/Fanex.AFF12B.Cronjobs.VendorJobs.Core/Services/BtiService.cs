using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public class BtiService : VendorService, IBtiService
    {
        private readonly IBtiApi _btiApi;
        private readonly IMemoryCache _memoryCache;
        private readonly BtiSetting _btiSetting;

        private const string BTI_TOKEN_MEMORY_CACHE = "BTI_TOKEN_MEMORY_CACHE";
        private const int BTI_CACHE_TIME_OUT = 5;
        private const string PRODUCT_NAME = "Bti";

        protected override string ErrorCodePropertyName => "errorCode";
        protected override string BetsPropertyName => "Bets";
        protected override string[] DataFieldNames => new string[] {
                "PurchaseID", "UserName", "TotalStake", "ValidStake",
                "PL", "BetStatus", "BetSettledDate",
            };

        public BtiService(
            ILogger logger,
            IBtiApi btiApi,
            IMemoryCache memoryCache,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<BtiSetting> btiSetting) : base(logger, trackerService, importDataRepository, btiSetting?.Value, PRODUCT_NAME)
        {
            _btiApi = btiApi ?? throw new ArgumentNullException(nameof(btiApi));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _btiSetting = btiSetting?.Value ?? throw new ArgumentNullException(nameof(btiSetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            var tokenInfo = await HandleGetTokenAsync();

            if (tokenInfo is null || !tokenInfo.IsValidToken)
            {
                throw ErrorMessageHelper.FormatException("BTi Product - cannot get a token!", param);
            }
            var rawData = await _btiApi.GetRawDataAsync(tokenInfo.Token, param);

            return new List<string> { rawData };
        }

        private async Task<BtiToken> HandleGetTokenAsync()
        {
            var token = await _memoryCache.GetOrCreateAsync(BTI_TOKEN_MEMORY_CACHE, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(BTI_CACHE_TIME_OUT);
                var tokenParam = new BtiTokenParam
                {
                    AgentUsername = _btiSetting.AgentUsername,
                    AgentPassword = _btiSetting.AgentPassword,
                };

                return await _btiApi.GetTokenAsync(tokenParam);
            });

            return token;
        }
    }
}
