using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;

using PortfolioManager.Common;
using PortfolioManager.Data.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{

    class SQLitePortfolioEntityCreator : IEntityCreator
    {
        private readonly Dictionary<Type, Func<SqliteDataReader, Entity>> _EntityCreators;

        public SQLitePortfolioEntityCreator()
        {
            _EntityCreators = new Dictionary<Type, Func<SqliteDataReader, Entity>>();

            _EntityCreators.Add(typeof(ShareParcel), CreateShareParcel);
            _EntityCreators.Add(typeof(Attachment), CreateAttachment);
            _EntityCreators.Add(typeof(CGTEvent), CreateCGTEvent);
            _EntityCreators.Add(typeof(CashAccountTransaction), CreateCashAccountTransaction);
            _EntityCreators.Add(typeof(ShareParcelAudit), CreateShareParcelAudit);
            _EntityCreators.Add(typeof(DRPCashBalance), CreateShareDRPCashBalance);
            _EntityCreators.Add(typeof(StockSetting), CreateStockSetting);
         
            _EntityCreators.Add(typeof(Aquisition), CreateAquisition);
            _EntityCreators.Add(typeof(CostBaseAdjustment), CreateCostBaseAdjustment);
            _EntityCreators.Add(typeof(Disposal), CreateDisposal);
            _EntityCreators.Add(typeof(IncomeReceived), CreateIncomeReceived);
            _EntityCreators.Add(typeof(OpeningBalance), CreateOpeningBalance);
            _EntityCreators.Add(typeof(ReturnOfCapital), CreateReturnOfCapital);
            _EntityCreators.Add(typeof(UnitCountAdjustment), CreateUnitCountAdjustment);
            _EntityCreators.Add(typeof(CashTransaction), CreateCashTransaction);
        }

        public T CreateEntity<T>(SqliteDataReader reader) where T : Entity
        {
            Func<SqliteDataReader, Entity> creationFunction;

            if (_EntityCreators.TryGetValue(typeof(T), out creationFunction))
                return (T)creationFunction(reader);
            else
                return default(T);
        }

        private ShareParcel CreateShareParcel(SqliteDataReader reader)
        {
            var shareParcel = new ShareParcel(new Guid(reader.GetString(0)), reader.GetDateTime(1), reader.GetDateTime(2))
            {
                Stock = new Guid(reader.GetString(3)),
                AquisitionDate = reader.GetDateTime(4),
                Units = reader.GetInt32(5),
                UnitPrice = SQLiteUtils.DBToDecimal(reader.GetInt64(6)),
                Amount = SQLiteUtils.DBToDecimal(reader.GetInt64(7)),
                CostBase = SQLiteUtils.DBToDecimal(reader.GetInt64(8)),
                PurchaseId = new Guid(reader.GetString(9))
            };

            return shareParcel;
        }

        private Attachment CreateAttachment(SqliteDataReader reader)
        {
            var attachment = new Attachment(new Guid(reader.GetString(0)))
            {
                Extension = reader.GetString(1)
            };

            byte[] buffer = new byte[1024];
            long bytesRead;
            long fieldOffset = 0;
            while ((bytesRead = reader.GetBytes(2, fieldOffset, buffer, 0, buffer.Length)) > 0)
            {
                attachment.Data.Write(buffer, 0, (int)bytesRead);
                fieldOffset += bytesRead;
            }
            attachment.Data.Seek(0, SeekOrigin.Begin);

            return attachment;
        }

        private CGTEvent CreateCGTEvent(SqliteDataReader reader)
        {
            var cgtEvent = new CGTEvent(new Guid(reader.GetString(0)))
            {
                Stock = new Guid(reader.GetString(1)),
                Units = reader.GetInt32(2),
                EventDate = reader.GetDateTime(3),
                CostBase = SQLiteUtils.DBToDecimal(reader.GetInt64(4)),
                AmountReceived = SQLiteUtils.DBToDecimal(reader.GetInt64(5)),
                CapitalGain = SQLiteUtils.DBToDecimal(reader.GetInt64(6)),
                CGTMethod = (CGTMethod)reader.GetInt32(7)
            };

            return cgtEvent;
        }

        private CashAccountTransaction CreateCashAccountTransaction(SqliteDataReader reader)
        {
            var cashTransaction = new CashAccountTransaction(new Guid(reader.GetString(0)))
            {
                Type = (BankAccountTransactionType)reader.GetInt32(1),
                Date = reader.GetDateTime(2),
                Description = reader.GetString(3),
                Amount = SQLiteUtils.DBToDecimal(reader.GetInt64(4)),
            };

            return cashTransaction;
        }

        private ShareParcelAudit CreateShareParcelAudit(SqliteDataReader reader)
        {
            var audit = new ShareParcelAudit(new Guid(reader.GetString(0)))
            {
                Parcel = new Guid(reader.GetString(1)),
                Date = reader.GetDateTime(2),
                Transaction = new Guid(reader.GetString(3)),
                UnitCount = reader.GetInt32(4),
                CostBaseChange = SQLiteUtils.DBToDecimal(reader.GetInt64(5)),
                AmountChange = SQLiteUtils.DBToDecimal(reader.GetInt64(6))
            };

            return audit;
        }

        private DRPCashBalance CreateShareDRPCashBalance(SqliteDataReader reader)
        {
            var balance = new DRPCashBalance(new Guid(reader.GetString(0)),
                                            reader.GetDateTime(1),
                                            reader.GetDateTime(2),
                                            SQLiteUtils.DBToDecimal(reader.GetInt64(3)));

            return balance;
        }

        private StockSetting CreateStockSetting(SqliteDataReader reader)
        {
            var setting = new StockSetting(new Guid(reader.GetString(0)), reader.GetDateTime(1), reader.GetDateTime(2))
            {
                ParticipateinDRP = SQLiteUtils.DBToBool(reader.GetString(3))
            };

            return setting;
        }

        private Aquisition CreateAquisition(SqliteDataReader reader)
        {
            var aquisition = new Aquisition(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                Units = reader.GetInt32(10),
                AveragePrice = SQLiteUtils.DBToDecimal(reader.GetInt64(11)),
                TransactionCosts = SQLiteUtils.DBToDecimal(reader.GetInt64(12)),
                CreateCashTransaction = SQLiteUtils.DBToBool(reader.GetString(13))
            };

            return aquisition;
        }

        private CostBaseAdjustment CreateCostBaseAdjustment(SqliteDataReader reader)
        {
            var costBaseAdjustment = new CostBaseAdjustment(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                Percentage = SQLiteUtils.DBToDecimal(reader.GetInt64(10))
            };

            return costBaseAdjustment;
        }

        private Disposal CreateDisposal(SqliteDataReader reader)
        {
            var disposal = new Disposal(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                Units = reader.GetInt32(10),
                AveragePrice = SQLiteUtils.DBToDecimal(reader.GetInt64(11)),
                TransactionCosts = SQLiteUtils.DBToDecimal(reader.GetInt64(12)),
                CGTMethod = (CGTCalculationMethod)reader.GetInt32(13),
                CreateCashTransaction = SQLiteUtils.DBToBool(reader.GetString(14))
            };

            return disposal;
        }

        private IncomeReceived CreateIncomeReceived(SqliteDataReader reader)
        {
            var incomeReceived = new IncomeReceived(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                FrankedAmount = SQLiteUtils.DBToDecimal(reader.GetInt64(10)),
                UnfrankedAmount = SQLiteUtils.DBToDecimal(reader.GetInt64(11)),
                FrankingCredits = SQLiteUtils.DBToDecimal(reader.GetInt64(12)),
                Interest = SQLiteUtils.DBToDecimal(reader.GetInt64(13)),
                TaxDeferred = SQLiteUtils.DBToDecimal(reader.GetInt64(14)),
                CreateCashTransaction = SQLiteUtils.DBToBool(reader.GetString(15)),
                DRPCashBalance = SQLiteUtils.DBToDecimal(reader.GetInt64(16))
            };

            return incomeReceived;
        }

        private OpeningBalance CreateOpeningBalance(SqliteDataReader reader)
        {
            var openingBalance = new OpeningBalance(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                Units = reader.GetInt32(10),
                CostBase = SQLiteUtils.DBToDecimal(reader.GetInt64(11)),
                AquisitionDate = reader.GetDateTime(12),
                PurchaseId = new Guid(reader.GetString(13))
            };

            return openingBalance;
        }

        private ReturnOfCapital CreateReturnOfCapital(SqliteDataReader reader)
        {
            var returnOfCapital = new ReturnOfCapital(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                Amount = SQLiteUtils.DBToDecimal(reader.GetInt64(10)),
                CreateCashTransaction = SQLiteUtils.DBToBool(reader.GetString(11))
            };

            return returnOfCapital;
        }

        private UnitCountAdjustment CreateUnitCountAdjustment(SqliteDataReader reader)
        {
            var unitCountAdjustment = new UnitCountAdjustment(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                OriginalUnits = reader.GetInt32(10),
                NewUnits = reader.GetInt32(11)
            };

            return unitCountAdjustment;
        }

        private CashTransaction CreateCashTransaction(SqliteDataReader reader)
        {
            var cashTransaction = new CashTransaction(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Attachment = new Guid(reader.GetString(6)),
                RecordDate = reader.GetDateTime(7),
                Comment = reader.GetString(8),
                CashTransactionType = (BankAccountTransactionType)reader.GetInt32(10),
                Amount = SQLiteUtils.DBToDecimal(reader.GetInt64(11))
            };

            return cashTransaction;
        }


    }

}

