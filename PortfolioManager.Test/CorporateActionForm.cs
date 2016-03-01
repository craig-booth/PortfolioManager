using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Stocks;
using StockManager.Service;

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
        private StockService _StockService;

        public CorporateActionFormFactory(StockService stockService)
        {
            _StockService = stockService;
        }

        public ICorporateActionForm CreateCorporateActionForm(CorporateActionType type)
        {
            if (type == CorporateActionType.Dividend)
                return new frmDividend(_StockService);
            else if (type == CorporateActionType.CapitalReturn)
                return new frmCapitalReturn(_StockService);
            else if (type == CorporateActionType.Transformation)
                return new frmTransformation(_StockService);
            else if (type == CorporateActionType.SplitConsolidation)
                return new frmSplitConsolidation(_StockService);
            else if (type == CorporateActionType.Composite)
                return new frmCompositeAction(_StockService);
            else
                throw new NotSupportedException();
        }
    }
}
