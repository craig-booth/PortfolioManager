using System;
using System.Collections.Generic;
using System.Text;

using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.Transactions;
using PortfolioManager.Domain.Utils;

namespace PortfolioManager.Domain.CorporateActions
{
    public class Transformation : CorporateAction
    {
        public DateTime ImplementationDate { get; private set; }
        public decimal CashComponent { get; private set; }
        public bool RolloverRefliefApplies { get; private set; }

        private List<ResultingStock> _ResultingStocks = new List<ResultingStock>();
        public IEnumerable<ResultingStock> ResultingStocks
        {
           get { return _ResultingStocks; }
        }

        internal Transformation(Guid id, Stock stock, DateTime actionDate, string description, DateTime implementationDate, decimal cashComponent, bool rolloverReliefApplies, IEnumerable<ResultingStock> resultingStocks)
            : base(id, stock, CorporateActionType.Transformation, actionDate, description)
        {
            ImplementationDate = implementationDate;
            CashComponent = cashComponent;
            RolloverRefliefApplies = rolloverReliefApplies;
            _ResultingStocks.AddRange(resultingStocks);
        }

        public class ResultingStock
        {
            public Guid Stock { get; private set; }
            public int OriginalUnits { get; set; }
            public int NewUnits { get; set; }
            public decimal CostBase { get; set; }
            public DateTime AquisitionDate { get; set; }

            public ResultingStock(Guid stock, int originalUnits, int newUnits, decimal costBase, DateTime aquisitionDate)
            {
                Stock = stock;
                OriginalUnits = originalUnits;
                NewUnits = newUnits;
                CostBase = costBase;
                AquisitionDate = aquisitionDate;
            }
        }

        public override IEnumerable<Transaction> GetTransactionList(Holding holding)
        {
            var transactions = new List<Transaction>();

            return transactions;
        }

        public override bool HasBeenApplied(ITransactionCollection transactions)
        {
            return false;
        }
    }
}
