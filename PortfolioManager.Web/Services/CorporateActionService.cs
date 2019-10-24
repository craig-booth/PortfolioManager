using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booth.Common;

using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.RestApi.CorporateActions;
using PortfolioManager.Web.Mappers;
using PortfolioManager.Web.Utilities;

namespace PortfolioManager.Web.Services
{
    public interface ICorporateActionService
    {
        RestApi.CorporateActions.CorporateAction GetCorporateAction(Guid stockId, Guid id);
        IEnumerable<RestApi.CorporateActions.CorporateAction> GetCorporateActions(Guid stockId, DateRange dateRange);

        void AddCorporateAction(Guid stockId, RestApi.CorporateActions.CorporateAction corporateAction);
    }

    public class CorporateActionService : ICorporateActionService
    {
        private readonly IStockQuery _StockQuery;
        private IRepository<Stock> _StockRepository;

        public CorporateActionService(IStockQuery stockQuery, IRepository<Stock> stockRepository)
        {
            _StockQuery = stockQuery;
            _StockRepository = stockRepository;
        }

        public RestApi.CorporateActions.CorporateAction GetCorporateAction(Guid stockId, Guid id)
        {
            var stock = _StockQuery.Get(stockId);
            if (stock == null)
                throw new StockNotFoundException(stockId);

            var corporateAction = stock.CorporateActions[id];
            if (corporateAction == null)
                throw new CorporateActionNotFoundException(id);

            return CorporateActionResponse(corporateAction);
        }

        public IEnumerable<RestApi.CorporateActions.CorporateAction> GetCorporateActions(Guid stockId, DateRange dateRange)
        {
            var stock = _StockQuery.Get(stockId);
            if (stock == null)
                throw new StockNotFoundException(stockId);

            return stock.CorporateActions.InDateRange(dateRange).Select(x => CorporateActionResponse(x)).ToList();
        }

        public void AddCorporateAction(Guid stockId, RestApi.CorporateActions.CorporateAction corporateAction)
        {
            var stock = _StockQuery.Get(stockId);
            if (stock == null)
                throw new StockNotFoundException(stockId);

            if (corporateAction is RestApi.CorporateActions.Dividend)
                AddDividend(stock, corporateAction as RestApi.CorporateActions.Dividend);
            else if (corporateAction is RestApi.CorporateActions.CapitalReturn)
                AddCapitalReturn(stock, corporateAction as RestApi.CorporateActions.CapitalReturn);
            else if (corporateAction is RestApi.CorporateActions.Transformation)
                AddTransformation(stock, corporateAction as RestApi.CorporateActions.Transformation);

            _StockRepository.Update(stock);
        }

        private CorporateAction CorporateActionResponse(Domain.CorporateActions.CorporateAction corporateAction)
        {
            if (corporateAction.Type == CorporateActionType.Dividend)
                return (corporateAction as Domain.CorporateActions.Dividend).ToResponse();
            else if (corporateAction.Type == CorporateActionType.CapitalReturn)
                return (corporateAction as Domain.CorporateActions.CapitalReturn).ToResponse();
            else if (corporateAction.Type == CorporateActionType.Transformation)
                return (corporateAction as Domain.CorporateActions.Transformation).ToResponse();
            else
                throw new UnknownCorporateActionType();
        }

        private void AddDividend(Stock stock, RestApi.CorporateActions.Dividend dividend)
        {
            stock.CorporateActions.AddDividend(dividend.Id, dividend.ActionDate, dividend.Description, dividend.PaymentDate, dividend.DividendAmount, dividend.PercentFranked, dividend.DRPPrice);
        }

        private void AddCapitalReturn(Stock stock, RestApi.CorporateActions.CapitalReturn capitalReturn)
        {
            stock.CorporateActions.AddCapitalReturn(capitalReturn.Id, capitalReturn.ActionDate, capitalReturn.Description, capitalReturn.PaymentDate, capitalReturn.Amount);
        }

        private void AddTransformation(Stock stock, RestApi.CorporateActions.Transformation transformation)
        {
            var resultingStocks = transformation.ResultingStocks.Select(x => new Domain.CorporateActions.Transformation.ResultingStock(x.Stock, x.OriginalUnits, x.NewUnits, x.CostBase, x.AquisitionDate));
            stock.CorporateActions.AddTransformation(transformation.Id, transformation.ActionDate, transformation.Description, transformation.ImplementationDate, transformation.CashComponent, transformation.RolloverRefliefApplies, resultingStocks);
        }
    }
}
