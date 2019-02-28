using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Stocks;

namespace PortfolioManager.Utils
{
    class CheckStockDividendRules
    {
        RestClient _RestClient;

        public CheckStockDividendRules()
        {
            var url = "https://docker.local:8443";
            //   var url = "http://localhost";
            var apiKey = new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D");
            var portfolioId = new Guid("5D5DE669-726C-4C5D-BB2E-6520C924DB90");

            _RestClient = new RestClient(url, apiKey, portfolioId);
        }

        public async Task CheckAll()
        {
            var stocks = await _RestClient.Stocks.Get(DateTime.Today);
            foreach (var stock in stocks)
            {
                await CheckStock(stock);
            }
        }
      
        public async Task CheckStock(StockResponse stock)
        {
            if (stock.CompanyTaxRate == 0.00m)
            {
                var changeCommand = new ChangeDividendRulesCommand()
                {
                    Id = stock.Id,
                    ChangeDate = stock.ListingDate,
                    CompanyTaxRate = 0.30m,
                    DividendRoundingRule = stock.DividendRoundingRule,
                    DRPActive = DRPActive(stock.ASXCode),
                    DRPMethod = DRPMethodUsed(stock.ASXCode)
                };

              //  await _RestClient.Stocks.ChangeDividendRules(changeCommand);
            }

        }

        private bool DRPActive(string asxCode)
        {
            if ((asxCode == "GVF") ||
                (asxCode == "ARG") ||
                (asxCode == "VGE") ||
                (asxCode == "VAP") ||
                (asxCode == "VCF") ||
                (asxCode == "WAM") ||
                (asxCode == "VGS") ||
                (asxCode == "VAF"))
                return true;
            else
                return false;
        }

        private DRPMethod DRPMethodUsed(string asxCode)
        {
            if ((asxCode == "VGE") ||
                (asxCode == "VAP") ||
                (asxCode == "VCF") ||
                (asxCode == "VGS") ||
                (asxCode == "VAF"))
                return DRPMethod.RetainCashBalance;
            else
                return DRPMethod.Round;
        }

    }
}
;