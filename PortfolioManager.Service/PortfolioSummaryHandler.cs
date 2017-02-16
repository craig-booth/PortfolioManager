using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Service
{

    class PortfolioSummaryHandler : IServiceHandler
    {
        private readonly ShareHoldingService _ShareHoldingService;
        private readonly CashAccountService _CashAccountService;

        public PortfolioSummaryHandler(ShareHoldingService shareHoldingService, CashAccountService cashAccountService)
        {
            _ShareHoldingService = shareHoldingService;
            _CashAccountService = cashAccountService;
        }

        public object HandleRequest(object request)
        {
            return HandleRequest((PortfolioSummaryRequest)request);
        }

        public PortfolioSummaryResponce HandleRequest(PortfolioSummaryRequest request)
        {
            var response = new PortfolioSummaryResponce();

            var holdings = _ShareHoldingService.GetHoldings(request.Date);
            var cashBalance = _CashAccountService.GetBalance(request.Date);

            response.PortfolioValue = holdings.Sum(x => x.MarketValue) + cashBalance; 
            response.PortfolioCost = holdings.Sum(x => x.TotalCostBase) + cashBalance;

            response.CashBalance = cashBalance;

            response.Return1Year = CalculateIRR(request.Date, 1);
            response.Return3Year = CalculateIRR(request.Date, 3);
            response.Return5Year = CalculateIRR(request.Date, 5);
            response.ReturnAll = CalculateIRR(request.Date, 0);

            foreach (var holding in holdings)
                response.Holdings.Add(new Holding()
                {
                    ASXCode = holding.Stock.ASXCode,
                    CompanyName = string.Format("{0} ({1})", holding.Stock.Name, holding.Stock.ASXCode),
                    Category = holding.Stock.Category,
                    Units = holding.Units,
                    Value = holding.MarketValue,
                    Cost = holding.TotalCostBase
                });

            return response;
        }

        private decimal? CalculateIRR(DateTime date, int years)
        {
            var portfolioStartDate = _ShareHoldingService.GetPortfolioStartDate();

            DateTime startDate;
            if (years == 0)
                startDate = portfolioStartDate;
            else
                startDate = date.AddYears(-years);

            if (startDate >= portfolioStartDate)
            {
                try
                {
                   return _ShareHoldingService.CalculateIRR(startDate, date);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            } 

        }


    }


    public class PortfolioSummaryRequest
    {
        public DateTime Date { get; set; }

        public PortfolioSummaryRequest(DateTime date)
        {
            Date = date;
        }
    }

    public class PortfolioSummaryResponce
    {
        public decimal PortfolioValue { get; set; }
        public decimal PortfolioCost { get; set; }

        public decimal? Return1Year { get; set; }
        public decimal? Return3Year { get; set; }
        public decimal? Return5Year { get; set; }
        public decimal? ReturnAll { get; set; }

        public decimal CashBalance { get; set; }

        public List<Holding> Holdings { get; private set; }

        public PortfolioSummaryResponce()
        {
            Holdings = new List<Holding>();
        }
    }

    public class Holding
    {
        public string ASXCode;
        public string CompanyName;

        public AssetCategory Category;

        public int Units;

        public decimal Value;
        public decimal Cost;
    }


}
