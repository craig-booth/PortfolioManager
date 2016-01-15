using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;


namespace PortfolioManager.Test
{
    public interface ICorporateActionForm
    {
        ICorporateAction CreateCorporateAction(Stock stock);
        bool EditCorporateAction(ICorporateAction corporateAction);
        void ViewCorporateAction(ICorporateAction corporateAction);
        Boolean DeleteCorporateAction(ICorporateAction corporateAction);
    }

    public class CorporateActionFormFactory
    {
        private StockManager _StockManager;

        public CorporateActionFormFactory(StockManager stockManager)
        {
            _StockManager = stockManager;
        }

        public ICorporateActionForm CreateCorporateActionForm(CorporateActionType type)
        {
            if (type == CorporateActionType.Dividend)
                return new frmDividend(_StockManager);
            else if (type == CorporateActionType.CapitalReturn)
                return new frmCapitalReturn(_StockManager);
            else if (type == CorporateActionType.Transformation)
                return new frmTransformation(_StockManager);
            else if (type == CorporateActionType.SplitConsolidation)
                return new frmSplitConsolidation(_StockManager);
            else
                throw new NotSupportedException();
        }
    }
}
