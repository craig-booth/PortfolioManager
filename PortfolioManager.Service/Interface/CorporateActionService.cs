using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Domain.CorporateActions;

namespace PortfolioManager.Service.Interface
{
    public interface ICorporateActionService 
    {
        Task<UnappliedCorporateActionsResponce> GetUnappliedCorporateActions();
        Task<TransactionsForCorparateActionsResponce> TransactionsForCorporateAction(Guid stockId, Guid actionId);
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
        public List<TransactionItem> Transactions { get; set; }

        public TransactionsForCorparateActionsResponce()
            : base()
        {
            Transactions = new List<TransactionItem>();
        }
    }
}
