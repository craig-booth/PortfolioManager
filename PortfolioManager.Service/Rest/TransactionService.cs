using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Service.Interface;

namespace PortfolioManager.Service.Rest
{
    class TransactionService : ITransactionService
    {
        private Local.LocalPortfolioManagerService _LocalService;
        private string _PortfolioDatabasePath;
        private string _StockDatabasePath;

        public TransactionService(Local.LocalPortfolioManagerService localService, string portfolioDatabasePath, string stockDatabasePath)
        {
            _LocalService = localService;
            _PortfolioDatabasePath = portfolioDatabasePath;
            _StockDatabasePath = stockDatabasePath;
        }

        public async Task<ServiceResponce> AddTransaction(TransactionItem transactionItem)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ITransactionService>();

            return await service.AddTransaction(transactionItem);
        }

        public async Task<ServiceResponce> AddTransactions(IEnumerable<TransactionItem> transactionItems)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ITransactionService>();

            return await service.AddTransactions(transactionItems);
        }

        public async Task<ServiceResponce> DeleteTransaction(TransactionItem transactionItem)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ITransactionService>();

            return await service.DeleteTransaction(transactionItem);
        }

        public async Task<GetTransactionsResponce> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ITransactionService>();

            return await service.GetTransactions(fromDate, toDate);
        }

        public async Task<GetTransactionsResponce> GetTransactions(Guid stockId, DateTime fromDate, DateTime toDate)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ITransactionService>();

            return await service.GetTransactions(stockId, fromDate, toDate);
        }

        public async Task<ServiceResponce> UpdateTransaction(TransactionItem transactionItem)
        {
            await _LocalService.Connect(_PortfolioDatabasePath, _StockDatabasePath);

            var service = _LocalService.GetService<ITransactionService>();

            return await service.UpdateTransaction(transactionItem);
        }

    }
}
