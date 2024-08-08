using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public class BetsoftService : VendorService, IBetsoftService
    {
        private readonly IBetsoftApi _betsoftApi;
        private readonly BetsoftSetting _betsoftSetting;
        private const string PRODUCT_NAME = "Betsoft";

        protected override string ErrorCodePropertyName => "status";
        protected override string TicketsPropertyName => "details";
        protected override string[] DataFieldNames => new string[] {
                "betOrderNo", "betTime", "account", "validBetAmount", "payout", "currency",
            };

        public BetsoftService(
            ILogger logger,
            IBetsoftApi betsoftApi,
            IVendorTrackerService trackerService,
            IImportVendorDataRepository importDataRepository,
            IOptions<BetsoftSetting> betsoftSetting) : base(logger, trackerService, importDataRepository, betsoftSetting?.Value, PRODUCT_NAME)
        {
            _betsoftApi = betsoftApi ?? throw new ArgumentNullException(nameof(betsoftApi));
            _betsoftSetting = betsoftSetting?.Value ?? throw new ArgumentNullException(nameof(betsoftSetting));
        }

        protected override async Task<IEnumerable<string>> GetRawDataAsync(BaseParam param)
        {
            if (_betsoftSetting.BankIds is null || _betsoftSetting.BankIds.Length == 0)
            {
                throw ErrorMessageHelper.FormatException($"{PRODUCT_NAME} Product Vendor - cannot get a bankid!", _betsoftSetting);
            }

            var rawDataList = new List<string>();
            foreach (var bankId in _betsoftSetting.BankIds)
            {
                var apiParam = BuildApiParam(param, bankId);
                var rawData = await _betsoftApi.GetRawDataAsync(apiParam);
                rawDataList.Add(rawData);
            }

            return rawDataList;
        }

        private BetsoftParam BuildApiParam(BaseParam param, string bankId)
        {
            var startDate = param.From.ToString(_betsoftSetting.FormatDateTime);
            var endDate = param.To.ToString(_betsoftSetting.FormatDateTime);

            return new BetsoftParam
            {
                StartDate = startDate,
                EndDate = endDate,
                MerchantCode = bankId
            };
        }
    }
}
