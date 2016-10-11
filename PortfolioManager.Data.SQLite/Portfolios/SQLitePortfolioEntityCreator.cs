using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;
using PortfolioManager.Data.SQLite.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLitePortfolioEntityCreator
    {
        public static Transaction CreateTransaction(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            Transaction transaction;

            Guid id = new Guid(reader.GetString(0));
            TransactionType type = (TransactionType)reader.GetInt32(3);

            if (type == TransactionType.Aquisition)
                transaction = CreateAquisition(database, id);
            else if (type == TransactionType.CostBaseAdjustment)
                transaction = CreateCostBaseAdjustment(database, id);
            else if (type == TransactionType.Disposal)
                transaction = CreateDisposal(database, id);
            else if (type == TransactionType.Income)
                transaction = CreateIncomeReceived(database, id);
            else if (type == TransactionType.OpeningBalance)
                transaction = CreateOpeningBalance(database, id);
            else if (type == TransactionType.ReturnOfCapital)
                transaction = CreateReturnOfCapital(database, id);
            else if (type == TransactionType.UnitCountAdjustment)
                transaction = CreateUnitCountAdjustment(database, id);
            else if (type == TransactionType.CashTransaction)
                transaction = CreateCashTransaction(database, id);
            else
                return null;

            transaction.TransactionDate = reader.GetDateTime(1);
            transaction.ASXCode = reader.GetString(4);
            transaction.Attachment = new Guid(reader.GetString(6));
            transaction.RecordDate = reader.GetDateTime(7);
            transaction.Comment = reader.GetString(8);

            return transaction;
        }

        private static Aquisition CreateAquisition(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get aquisition values */
            var command = new SQLiteCommand("SELECT [Units], [AveragePrice], [TransactionCosts], [CreateCashTransaction] FROM [Aquisitions] WHERE [Id] = @Id", database._Connection);
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

        private static CostBaseAdjustment CreateCostBaseAdjustment(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get cost base adjsutment values */
            var command = new SQLiteCommand("SELECT [Percentage] FROM [CostBaseAdjustments] WHERE [Id] = @Id", database._Connection);
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

        private static Disposal CreateDisposal(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get disposal values */
            var command = new SQLiteCommand("SELECT [Units], [AveragePrice], [TransactionCosts], [CGTMethod], [CreateCashTransaction] FROM [Disposals] WHERE [Id] = @Id", database._Connection);
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

        private static IncomeReceived CreateIncomeReceived(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get income received values */
            var command = new SQLiteCommand("SELECT [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [CreateCashTransaction] FROM [IncomeReceived] WHERE [Id] = @Id", database._Connection);
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
                CreateCashTransaction = SQLiteUtils.DBToBool(incomeReceivedReader.GetString(5))
            };
            incomeReceivedReader.Close();

            return incomeReceived;
        }

        private static OpeningBalance CreateOpeningBalance(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Units], [CostBase], [AquisitionDate], [PurchaseId] FROM [OpeningBalances] WHERE [Id] = @Id", database._Connection);
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

        private static ReturnOfCapital CreateReturnOfCapital(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Amount], [CreateCashTransaction] FROM [ReturnsOfCapital] WHERE [Id] = @Id", database._Connection);
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

        private static UnitCountAdjustment CreateUnitCountAdjustment(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [OriginalUnits], [NewUnits] FROM [UnitCountAdjustments] WHERE [Id] = @Id", database._Connection);
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

        private static CashTransaction CreateCashTransaction(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Type], [Amount] FROM [CashTransactions] WHERE [Id] = @Id", database._Connection);
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
                CashTransactionType = (CashAccountTransactionType)cashTransactionReader.GetInt32(0),
                Amount = SQLiteUtils.DBToDecimal(cashTransactionReader.GetInt64(1))
            };

            cashTransactionReader.Close();

            return cashTransaction;
        }
    }
}

