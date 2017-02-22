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

    public class UnappliedCorporateActionsResponce
    {
        public List<CorporateActionItem> CorporateActions { get; set; }

        public UnappliedCorporateActionsResponce()
        {
            CorporateActions = new List<CorporateActionItem>();
        }
    }

    public class CorporateActionItem
    {
        public Guid Id { get; set; }
        public DateTime ActionDate { get; set; }
        public string ASXCode { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
    }

    public class TransactionsForCorparateActionsResponce
    {
        public List<Transaction> Transactions { get; set; }

        public TransactionsForCorparateActionsResponce()
        {
            Transactions = new List<Transaction>();
        }
    }
}
