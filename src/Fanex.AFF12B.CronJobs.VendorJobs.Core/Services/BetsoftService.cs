using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services
{
    public interface IBetsoftService : IVendorService { }

    public class BetsoftService : VendorService, IBetsoftService
    {
        private readonly IBetsoftApi _betsoftApi;
        private readonly BetsoftSetting _betsoftSetting;
        private const string PRODUCT_NAME = "Betsoft";
        private const string RESPONSE_PROPERTY_NAME = "RESPONSE";

        protected override string ErrorCodePropertyName => RESPONSE_PROPERTY_NAME;
        protected override string TicketsPropertyName => "ACCOUNT";
        protected override bool IsXml => true;

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

            var inputToHash = $"{startDate}{endDate}{bankId}{_betsoftSetting.Password}";

            return new BetsoftParam
            {
                StartDate = startDate,
                EndDate = endDate,
                BankId = bankId,
                Hash = inputToHash.Md5()
            };
        }

        protected override bool ShouldImportToDb(XElement root)
        {
            return root.Elements(RESPONSE_PROPERTY_NAME).Elements(TicketsPropertyName).Count() != 0;
        }

        protected override bool IsValidXml(XElement element)
        {
            return StatusCode.Ok.IsMatch(element.Element("RESULT").Value);
        }
    }
}
