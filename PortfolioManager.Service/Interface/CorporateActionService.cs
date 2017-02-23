using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Service.Interface
{
    public interface ICorporateActionService : IPortfolioManagerService
    {
        Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions();
        Task<TransactionsForCorparateActionsResponce> TransactionsForCorporateAction(Guid corporateAction);
    }

    public class UnappliedCorporateActionsResponce : ServiceResponce
    {
        public List<CorporateActionItem> CorporateActions { get; set; }

        public UnappliedCorporateActionsResponce()
            : base()
        {
            CorporateActions = new List<CorporateActionItem>();
        }
    }

    public class CorporateActionItem
    {
        public Guid Id { get; set; }
        public DateTime ActionDate { get; set; }
        public StockItem Stock { get; set; }
        public string Description { get; set; }
    }

    public class TransactionsForCorparateActionsResponce: ServiceResponce
    {
        public List<Transaction> Transactions { get; set; }

        public TransactionsForCorparateActionsResponce()
            : base()
        {
            Transactions = new List<Transaction>();
        }
    }
}
