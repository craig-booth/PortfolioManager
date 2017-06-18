using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Data;

namespace PortfolioManager.Data.Stocks
{

    public class Transformation : CorporateAction
    {
        public DateTime ImplementationDate { get; set; }
        public decimal CashComponent { get; set; }
        public bool RolloverRefliefApplies { get; set; }
        public List<ResultingStock> ResultingStocks { get; private set; }


        public Transformation(Guid id, Guid stock, DateTime actionDate, DateTime implementationDate, decimal cashComponent, bool rolloverReliefApplies, string description)
            : base(id, CorporateActionType.Transformation, stock, actionDate)
        {
            ImplementationDate = implementationDate;
            CashComponent = cashComponent;
            RolloverRefliefApplies = rolloverReliefApplies;
            Description = description;

            ResultingStocks = new List<ResultingStock>();
        }

        public Transformation(Guid stock, DateTime actionDate, DateTime implementationDate, decimal cashComponent, bool rolloverReliefApplies, string description)
            : this(Guid.NewGuid(), stock, actionDate, implementationDate, cashComponent, rolloverReliefApplies, description)
        {

        }

        public ResultingStock GetResultStock(Guid id)
        {
            return ResultingStocks.Find(x => x.Stock == id);
        }

        public void AddResultStock(ResultingStock resultingStock)
        {
            ResultingStocks.Add(resultingStock);
        }

        public void AddResultStock(Guid stock, int originalUnits, int newUnits, decimal costBasePercentage)
        {
            var resultStock = new ResultingStock(stock, originalUnits, newUnits, costBasePercentage);

            ResultingStocks.Add(resultStock);
        }

        public void AddResultStock(Guid stock, int originalUnits, int newUnits, decimal unitCostBase, DateTime aquisitionDate)
        {
            var resultStock = new ResultingStock(stock, originalUnits, newUnits, unitCostBase, aquisitionDate);

            ResultingStocks.Add(resultStock);
        }
        
        public void DeleteResultStock(Guid stock)
        {
            var resultStock = GetResultStock(stock);

            if (resultStock == null)
                throw new RecordNotFoundException(stock);

            ResultingStocks.Remove(resultStock);
        }
    }

    public class ResultingStock
    {
        public Guid Stock { get; private set; }
        public int OriginalUnits { get; set; }
        public int NewUnits { get; set; }
        public decimal CostBase { get; set; }
        public DateTime AquisitionDate { get; set; }

        public ResultingStock(Guid stock, int originalUnits, int newUnits, decimal costBasePercentage) :
            this(stock, originalUnits, newUnits, costBasePercentage, DateUtils.NoDate)
        {
        }

        public ResultingStock(Guid stock, int originalUnits, int newUnits, decimal costBase, DateTime aquisitionDate)
        {
            Stock = stock;
            OriginalUnits = originalUnits;
            NewUnits = newUnits;
            CostBase = costBase;
            AquisitionDate = aquisitionDate;
        }

    }


}
