using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLitePortfolioQuery : SQLiteQuery, IPortfolioQuery
    {
        private SQLitePortfolioDatabase _Database;

        protected internal SQLitePortfolioQuery(SQLitePortfolioDatabase database)
            : base(database._Connection, new SQLitePortfolioEntityCreator(database))
        {
            _Database = database;
        }

        public ShareParcel GetParcel(Guid id, DateTime atDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                .WithId(id)
                .EffectiveAt(atDate);

            return query.CreateEntity<ShareParcel>();
        }

        public IEnumerable<ShareParcel> GetParcels(Guid id, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                            .WithId(id)
                            .EffectiveBetween(fromDate, toDate);

            return query.CreateEntities<ShareParcel>();
        }

        public IEnumerable<ShareParcel> GetAllParcels(DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                            .EffectiveBetween(fromDate, toDate);

            return query.CreateEntities<ShareParcel>();
        }


        public IEnumerable<ShareParcel> GetParcelsForStock(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Parcels")
                .Where("[Stock] = @Stock")
                .WithParameter("@Stock", stock)
                .EffectiveBetween(fromDate, toDate);

            return query.CreateEntities<ShareParcel>();
        }

        public bool StockOwned(Guid id, DateTime atDate)
        {
            bool result;

            var query = EntityQuery.FromTable("Parcels")
                    .Select("Id")
                    .Where("[Stock] = @Stock")
                    .WithParameter("@Stock", id)
                    .EffectiveAt(atDate);

            var reader = query.ExecuteSingle();
            result = reader.HasRows;

            reader.Close();

            return result;
        }

        public IEnumerable<Guid> GetStocksOwned(DateTime fromDate, DateTime toDate)
        {
            List<Guid> stocks = new List<Guid>();

            var query = EntityQuery.FromTable("Parcels")
                    .Select("DISTINCT [Stock]")
                    .EffectiveBetween(fromDate, toDate);

            var reader = query.Execute();
            while (reader.Read())
            {
                stocks.Add(new Guid(reader.GetString(0)));
            }

            reader.Close();

            return stocks;
        }

        public IEnumerable<CGTEvent> GetCGTEvents(DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("CGTEvents")
                            .Where("[EventDate] between @FromDate and @ToDate")
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate)
                            .OrderBy("[EventDate]");

            return query.CreateEntities<CGTEvent>();
        }

        public Transaction GetTransaction(Guid id)
        {
            var query = EntityQuery.FromTable("Transactions")
                .WithId(id);

            return query.CreateEntity<Transaction>();
        }

        public IEnumerable<Transaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                            .Where("[TransactionDate] between @FromDate and @ToDate")
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate)
                            .OrderBy("[RecordDate], [Sequence]");

            return query.CreateEntities<Transaction>();
        }

        public IEnumerable<Transaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                            .Where("[Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate")
                            .OrderBy("[RecordDate], [Sequence]")
                            .WithParameter("@Type", (int)transactionType)
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate);

            return query.CreateEntities<Transaction>();
        }

        public IEnumerable<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                            .Where("[ASXCode] = @ASXCode AND [TransactionDate] BETWEEN @FromDate AND @ToDate")
                            .OrderBy("[RecordDate], [Sequence]")
                            .WithParameter("@ASXCode", asxCode)
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate);

            return query.CreateEntities<Transaction>();
        }

        public IEnumerable<Transaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                .Where("[ASXCode] = @ASXCode AND [Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate")
                .OrderBy("[RecordDate], [Sequence]")
                .WithParameter("@ASXCode", asxCode)
                .WithParameter("@Type", (int)transactionType)
                .WithParameter("@FromDate", fromDate)
                .WithParameter("@ToDate", toDate);

            return query.CreateEntities<Transaction>();
        }

        public decimal GetCashBalance(DateTime atDate)
        {
            var query = EntityQuery.FromTable("CashAccountTransactions")
                            .Select("SUM([Amount])")
                            .Where("[Date] <= @Date")
                            .WithParameter("@Date", atDate);

            decimal balance;
            query.ExecuteScalar(out balance);

            return balance;
        }

        public IEnumerable<CashAccountTransaction> GetCashAccountTransactions(DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("CashAccountTransactions")
                            .Where("[Date] between @FromDate and @ToDate")
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate)
                            .OrderBy("[Date], [Amount] DESC");

            return query.CreateEntities<CashAccountTransaction>();
        }

        public IEnumerable<ShareParcelAudit> GetParcelAudit(Guid id, DateTime fromDate, DateTime toDate)
        {
            return _Database._ParcelAudit.Where(x => (x.Parcel == id) && (x.Date >= fromDate) && (x.Date <= toDate)).ToList();
        }

        public StockSetting GetStockSetting(Guid stock, DateTime atDate)
        {
            var stockSettingQuery = from stockSetting in _Database._StockSettings
                                  where (stockSetting.Id == stock) && ((atDate >= stockSetting.FromDate && atDate <= stockSetting.ToDate))
                                  select stockSetting;

            return stockSettingQuery.FirstOrDefault();
        }

        public decimal GetDRPCashBalance(Guid stock, DateTime atDate)
        {
            var drpBalanceQuery = from drpBalance in _Database._DRPCashBalances
                               where (drpBalance.Id == stock) && ((atDate >= drpBalance.FromDate && atDate <= drpBalance.ToDate))
                               select drpBalance.Balance;

            return drpBalanceQuery.FirstOrDefault();
        }

    }
}
