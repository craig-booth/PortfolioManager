using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{

    class SQLitePortfolioEntityCreator : IEntityCreator
    {
        private readonly SQLitePortfolioDatabase _Database;
        private readonly Dictionary<Type, Func<SQLiteDataReader, Entity>> _EntityCreators;

        public SQLitePortfolioEntityCreator(SQLitePortfolioDatabase database)
        {
            _Database = database;
            _EntityCreators = new Dictionary<Type, Func<SQLiteDataReader, Entity>>();

            _EntityCreators.Add(typeof(Transaction), CreateTransaction);
            _EntityCreators.Add(typeof(ShareParcel), CreateShareParcel);
            _EntityCreators.Add(typeof(Attachment), CreateAttachment);
            _EntityCreators.Add(typeof(CGTEvent), CreateCGTEvent);
            _EntityCreators.Add(typeof(CashAccountTransaction), CreateCashAccountTransaction);
            _EntityCreators.Add(typeof(ShareParcelAudit), CreateShareParcelAudit);
            _EntityCreators.Add(typeof(DRPCashBalance), CreateShareDRPCashBalance);
            _EntityCreators.Add(typeof(StockSetting), CreateStockSetting);
        }

        public T CreateEntity<T>(SQLiteDataReader reader) where T : Entity
        {
            Func<SQLiteDataReader, Entity> creationFunction;

            if (_EntityCreators.TryGetValue(typeof(T), out creationFunction))
                return (T)creationFunction(reader);
            else
                return default(T);
        }

        private Transaction CreateTransaction(SQLiteDataReader reader)
        {
            Transaction transaction;

            Guid id = new Guid(reader.GetString(0));
            TransactionType type = (TransactionType)reader.GetInt32(3);

            if (type == TransactionType.Aquisition)
                transaction = CreateAquisition(id);
            else if (type == TransactionType.CostBaseAdjustment)
                transaction = CreateCostBaseAdjustment(id);
            else if (type == TransactionType.Disposal)
                transaction = CreateDisposal(id);
            else if (type == TransactionType.Income)
                transaction = CreateIncomeReceived(id);
            else if (type == TransactionType.OpeningBalance)
                transaction = CreateOpeningBalance(id);
            else if (type == TransactionType.ReturnOfCapital)
                transaction = CreateReturnOfCapital(id);
            else if (type == TransactionType.UnitCountAdjustment)
                transaction = CreateUnitCountAdjustment(id);
            else if (type == TransactionType.CashTransaction)
                transaction = CreateCashTransaction(id);
            else
                return null;

            transaction.TransactionDate = reader.GetDateTime(1);
            transaction.ASXCode = reader.GetString(4);
            transaction.Attachment = new Guid(reader.GetString(6));
            transaction.RecordDate = reader.GetDateTime(7);
            transaction.Comment = reader.GetString(8);

            return transaction;
        }

        private ShareParcel CreateShareParcel(SQLiteDataReader reader)
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

        private Attachment CreateAttachment(SQLiteDataReader reader)
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

        private CGTEvent CreateCGTEvent(SQLiteDataReader reader)
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

        private CashAccountTransaction CreateCashAccountTransaction(SQLiteDataReader reader)
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

        private ShareParcelAudit CreateShareParcelAudit(SQLiteDataReader reader)
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

        private DRPCashBalance CreateShareDRPCashBalance(SQLiteDataReader reader)
        {
            var balance = new DRPCashBalance(new Guid(reader.GetString(0)),
                                            reader.GetDateTime(1),
                                            reader.GetDateTime(2),
                                            SQLiteUtils.DBToDecimal(reader.GetInt64(3)));

            return balance;
        }

        private StockSetting CreateStockSetting(SQLiteDataReader reader)
        {
            var setting = new StockSetting(new Guid(reader.GetString(0)), reader.GetDateTime(1), reader.GetDateTime(2))
            {
                ParticipateinDRP = SQLiteUtils.DBToBool(reader.GetString(3))
            };

            return setting;
        }

        private Aquisition CreateAquisition(Guid id)
        {
            /* Get aquisition values */
            var command = new SQLiteCommand("SELECT [Units], [AveragePrice], [TransactionCosts], [CreateCashTransaction] FROM [Aquisitions] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader aquisitionReader = command.ExecuteReader();
            if (!aquisitionReader.Read())
            {
                aquisitionReader.Close();
                throw new RecordNotFoundException(id);
            }

            Aquisition aquisition = new Aquisition(id)
            {
                Units = aquisitionReader.GetInt32(0),
                AveragePrice = SQLiteUtils.DBToDecimal(aquisitionReader.GetInt64(1)),
                TransactionCosts = SQLiteUtils.DBToDecimal(aquisitionReader.GetInt64(2)),
                CreateCashTransaction = SQLiteUtils.DBToBool(aquisitionReader.GetString(3))
            };
            aquisitionReader.Close();

            return aquisition;
        }

        private CostBaseAdjustment CreateCostBaseAdjustment(Guid id)
        {
            /* Get cost base adjsutment values */
            var command = new SQLiteCommand("SELECT [Percentage] FROM [CostBaseAdjustments] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader costBaseAdjustmentReader = command.ExecuteReader();
            if (!costBaseAdjustmentReader.Read())
            {
                costBaseAdjustmentReader.Close();
                throw new RecordNotFoundException(id);
            }

            CostBaseAdjustment costBaseAdjustment = new CostBaseAdjustment(id)
            {
                Percentage = SQLiteUtils.DBToDecimal(costBaseAdjustmentReader.GetInt64(0))
            };
            costBaseAdjustmentReader.Close();

            return costBaseAdjustment;
        }

        private Disposal CreateDisposal(Guid id)
        {
            /* Get disposal values */
            var command = new SQLiteCommand("SELECT [Units], [AveragePrice], [TransactionCosts], [CGTMethod], [CreateCashTransaction] FROM [Disposals] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader disposalReader = command.ExecuteReader();
            if (!disposalReader.Read())
            {
                disposalReader.Close();
                throw new RecordNotFoundException(id);
            }

            Disposal disposal = new Disposal(id)
            {
                Units = disposalReader.GetInt32(0),
                AveragePrice = SQLiteUtils.DBToDecimal(disposalReader.GetInt64(1)),
                TransactionCosts = SQLiteUtils.DBToDecimal(disposalReader.GetInt64(2)),
                CGTMethod = (CGTCalculationMethod)disposalReader.GetInt32(3),
                CreateCashTransaction = SQLiteUtils.DBToBool(disposalReader.GetString(4))
            };
            disposalReader.Close();

            return disposal;
        }

        private IncomeReceived CreateIncomeReceived(Guid id)
        {
            /* Get income received values */
            var command = new SQLiteCommand("SELECT [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [CreateCashTransaction], [DRPCashBalance] FROM [IncomeReceived] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader incomeReceivedReader = command.ExecuteReader();
            if (!incomeReceivedReader.Read())
            {
                incomeReceivedReader.Close();
                throw new RecordNotFoundException(id);
            }

            IncomeReceived incomeReceived = new IncomeReceived(id)
            {
                FrankedAmount = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt64(0)),
                UnfrankedAmount = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt64(1)),
                FrankingCredits = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt64(2)),
                Interest = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt64(3)),
                TaxDeferred = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt64(4)),
                CreateCashTransaction = SQLiteUtils.DBToBool(incomeReceivedReader.GetString(5)),
                DRPCashBalance = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt64(6))
            };
            incomeReceivedReader.Close();

            return incomeReceived;
        }

        private OpeningBalance CreateOpeningBalance(Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Units], [CostBase], [AquisitionDate], [PurchaseId] FROM [OpeningBalances] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader openingBalanceReader = command.ExecuteReader();
            if (!openingBalanceReader.Read())
            {
                openingBalanceReader.Close();
                throw new RecordNotFoundException(id);
            }

            OpeningBalance openingBalance = new OpeningBalance(id)
            {
                Units = openingBalanceReader.GetInt32(0),
                CostBase = SQLiteUtils.DBToDecimal(openingBalanceReader.GetInt64(1)),
                AquisitionDate = openingBalanceReader.GetDateTime(2),
                PurchaseId = new Guid(openingBalanceReader.GetString(3))
            };

            openingBalanceReader.Close();

            return openingBalance;
        }

        private ReturnOfCapital CreateReturnOfCapital(Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Amount], [CreateCashTransaction] FROM [ReturnsOfCapital] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader returnOfCapitalReader = command.ExecuteReader();
            if (!returnOfCapitalReader.Read())
            {
                returnOfCapitalReader.Close();
                throw new RecordNotFoundException(id);
            }

            ReturnOfCapital returnOfCapital = new ReturnOfCapital(id)
            {
                Amount = SQLiteUtils.DBToDecimal(returnOfCapitalReader.GetInt64(0)),
                CreateCashTransaction = SQLiteUtils.DBToBool(returnOfCapitalReader.GetString(1))
            };
            returnOfCapitalReader.Close();

            return returnOfCapital;
        }

        private UnitCountAdjustment CreateUnitCountAdjustment(Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [OriginalUnits], [NewUnits] FROM [UnitCountAdjustments] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader unitCountAdjustmentReader = command.ExecuteReader();
            if (!unitCountAdjustmentReader.Read())
            {
                unitCountAdjustmentReader.Close();
                throw new RecordNotFoundException(id);
            }

            UnitCountAdjustment unitCountAdjustmnet = new UnitCountAdjustment(id)
            {
                OriginalUnits = unitCountAdjustmentReader.GetInt32(0),
                NewUnits = unitCountAdjustmentReader.GetInt32(1)
            };

            unitCountAdjustmentReader.Close();

            return unitCountAdjustmnet;
        }

        private CashTransaction CreateCashTransaction(Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Type], [Amount] FROM [CashTransactions] WHERE [Id] = @Id", _Database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader cashTransactionReader = command.ExecuteReader();
            if (!cashTransactionReader.Read())
            {
                cashTransactionReader.Close();
                throw new RecordNotFoundException(id);
            }

            CashTransaction cashTransaction = new CashTransaction(id)
            {
                CashTransactionType = (BankAccountTransactionType)cashTransactionReader.GetInt32(0),
                Amount = SQLiteUtils.DBToDecimal(cashTransactionReader.GetInt64(1))
            };

            cashTransactionReader.Close();

            return cashTransaction;
        }

    }

}

