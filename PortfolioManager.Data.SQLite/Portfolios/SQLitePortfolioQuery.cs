using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLitePortfolioQuery : SQLiteQuery, IPortfolioQuery
    {
     
        protected internal SQLitePortfolioQuery(SqliteTransaction transaction)
            : base(transaction, new SQLitePortfolioEntityCreator())
        {
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
            var query = EntityQuery.FromTable("Parcels")
                    .Select("Id")
                    .Where("[Stock] = @Stock")
                    .WithParameter("@Stock", id)
                    .EffectiveAt(atDate);

            using (var reader = query.ExecuteSingle())
            {
                return reader.HasRows;
            }
        }

        public IEnumerable<Guid> GetStocksOwned(DateTime fromDate, DateTime toDate)
        {
            List<Guid> stocks = new List<Guid>();

            var query = EntityQuery.FromTable("Parcels")
                    .Select("DISTINCT [Stock]")
                    .EffectiveBetween(fromDate, toDate);

            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    stocks.Add(new Guid(reader.GetString(0)));
                }
            }

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
                .Select("[Id], [Type]")
                .WithId(id);

            using (var reader = query.Execute())
            {
                if (reader.Read())
                    return GetTransaction(new Guid(reader.GetString(0)), (TransactionType)reader.GetInt32(1));
                else
                    return null;
            }
        }

        private T GetTransaction<T>(Guid id) where T : Transaction
        {
            var query = EntityQuery.FromTable("Transactions")
                            .WithId(id);

            if (typeof(T) == typeof(Aquisition))
                query.Join("Aquisitions", "Transactions.Id = Aquisitions.Id");
            else if (typeof(T) == typeof(CashTransaction))
                query.Join("CashTransactions", "Transactions.Id = CashTransactions.Id");
            else if (typeof(T) == typeof(CostBaseAdjustment))
                query.Join("CostBaseAdjustments", "Transactions.Id = CostBaseAdjustments.Id");
            else if (typeof(T) == typeof(Disposal))
                query.Join("Disposals", "Transactions.Id = Disposals.Id");
            else if (typeof(T) == typeof(IncomeReceived))
                query.Join("IncomeReceived", "Transactions.Id = IncomeReceived.Id");
            else if (typeof(T) == typeof(OpeningBalance))
                query.Join("OpeningBalances", "Transactions.Id = OpeningBalances.Id");
            else if (typeof(T) == typeof(ReturnOfCapital))
                query.Join("ReturnsOfCapital", "Transactions.Id = ReturnsOfCapital.Id");
            else if (typeof(T) == typeof(UnitCountAdjustment))
                query.Join("UnitCountAdjustments", "Transactions.Id = UnitCountAdjustments.Id");

            return query.CreateEntity<T>();
        }

        private Transaction GetTransaction(Guid id, TransactionType type)
        {          
            if (type == TransactionType.Aquisition)
                return GetTransaction<Aquisition>(id);
            else if (type == TransactionType.CashTransaction)
                return GetTransaction<CashTransaction>(id);
            else if (type == TransactionType.CostBaseAdjustment)
                return GetTransaction<CostBaseAdjustment>(id);
            else if (type == TransactionType.Disposal)
                return GetTransaction<Disposal>(id);
            else if (type == TransactionType.Income)
                return GetTransaction<IncomeReceived>(id);
            else if (type == TransactionType.OpeningBalance)
                return GetTransaction<OpeningBalance>(id);
            else if (type == TransactionType.ReturnOfCapital)
                return GetTransaction<ReturnOfCapital>(id);
            else if (type == TransactionType.UnitCountAdjustment)
                return GetTransaction<UnitCountAdjustment>(id);
            else
                return null;
            
        }

        public IEnumerable<Transaction> GetTransactions(DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                            .Select("[Id], [Type]")
                            .Where("[TransactionDate] between @FromDate and @ToDate")
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate)
                            .OrderBy("[RecordDate], [Sequence]");

            var transactions = new List<Transaction>();
            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    transactions.Add(GetTransaction(new Guid(reader.GetString(0)), (TransactionType)reader.GetInt32(1)));
                }
            }

            return transactions;
        }

        public IEnumerable<Transaction> GetTransactions(TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                            .Select("[Id], [Type]")
                            .Where("[Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate")
                            .OrderBy("[RecordDate], [Sequence]")
                            .WithParameter("@Type", (int)transactionType)
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate);

            var transactions = new List<Transaction>();
            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    transactions.Add(GetTransaction(new Guid(reader.GetString(0)), (TransactionType)reader.GetInt32(1)));
                }
            }

            return transactions;
        }

        public IEnumerable<Transaction> GetTransactions(string asxCode, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                            .Select("[Id], [Type]")
                            .Where("[ASXCode] = @ASXCode AND [TransactionDate] BETWEEN @FromDate AND @ToDate")
                            .OrderBy("[RecordDate], [Sequence]")
                            .WithParameter("@ASXCode", asxCode)
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate);

            var transactions = new List<Transaction>();
            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    transactions.Add(GetTransaction(new Guid(reader.GetString(0)), (TransactionType)reader.GetInt32(1)));
                }
            }

            return transactions;
        }

        public IEnumerable<Transaction> GetTransactions(string asxCode, TransactionType transactionType, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("Transactions")
                .Select("[Id], [Type]")
                .Where("[ASXCode] = @ASXCode AND [Type] = @Type AND [TransactionDate] BETWEEN @FromDate AND @ToDate")
                .OrderBy("[RecordDate], [Sequence]")
                .WithParameter("@ASXCode", asxCode)
                .WithParameter("@Type", (int)transactionType)
                .WithParameter("@FromDate", fromDate)
                .WithParameter("@ToDate", toDate);

            var transactions = new List<Transaction>();
            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    transactions.Add(GetTransaction(new Guid(reader.GetString(0)), (TransactionType)reader.GetInt32(1)));
                }
            }

            return transactions;
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
            var query = EntityQuery.FromTable("ParcelAudit")
                            .Where("[Parcel] = @Parcel and [Date] between @FromDate and @ToDate")
                            .WithParameter("@Parcel", id)
                            .WithParameter("@FromDate", fromDate)
                            .WithParameter("@ToDate", toDate);

            return query.CreateEntities<ShareParcelAudit>();
        }

        public StockSetting GetStockSetting(Guid stock, DateTime atDate)
        {
            var query = EntityQuery.FromTable("StockSettings")
                            .WithId(stock)
                            .EffectiveAt(atDate);

            return query.CreateEntity<StockSetting>();
        }


        public DRPCashBalance GetDRPCashBalance(Guid stock, DateTime atDate)
        {
            var query = EntityQuery.FromTable("DRPCashBalances")
                            .WithId(stock)
                            .EffectiveAt(atDate);

            return query.CreateEntity<DRPCashBalance>();
        }

        public decimal GetDRPBalance(Guid stock, DateTime atDate)
        {
            var query = EntityQuery.FromTable("DRPCashBalances")
                            .Select("Balance")
                            .WithId(stock)
                            .EffectiveAt(atDate);

            decimal balance;
            query.ExecuteScalar(out balance);

            return balance;
        }

    }
  
}
