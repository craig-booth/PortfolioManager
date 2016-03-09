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
            else
                return null;

            transaction.TransactionDate = reader.GetDateTime(1);
            transaction.ASXCode = reader.GetString(4);
            transaction.Attachment = new Guid(reader.GetString(6));

            return transaction;
        }

        private static Aquisition CreateAquisition(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get aquisition values */
            var command = new SQLiteCommand("SELECT [Units],[AveragePrice], [TransactionCosts], [Comment] FROM [Aquisitions] WHERE [Id] = @Id", database._Connection);
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
                AveragePrice = SQLiteUtils.DBToDecimal(aquisitionReader.GetInt32(1)),
                TransactionCosts = SQLiteUtils.DBToDecimal(aquisitionReader.GetInt32(2)),
                Comment = aquisitionReader.GetString(3)
            };
            aquisitionReader.Close();

            return aquisition;
        }

        private static CostBaseAdjustment CreateCostBaseAdjustment(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get cost base adjsutment values */
            var command = new SQLiteCommand("SELECT [RecordDate], [Percentage], [Comment] FROM [CostBaseAdjustments] WHERE [Id] = @Id", database._Connection);
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
                RecordDate = costBaseAdjustmentReader.GetDateTime(0),
                Percentage = SQLiteUtils.DBToDecimal(costBaseAdjustmentReader.GetInt32(1)),
                Comment = costBaseAdjustmentReader.GetString(2)
            };
            costBaseAdjustmentReader.Close();

            return costBaseAdjustment;
        }

        private static Disposal CreateDisposal(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get disposal values */
            var command = new SQLiteCommand("SELECT [Units], [AveragePrice], [TransactionCosts], [CGTMethod], [Comment] FROM [Disposals] WHERE [Id] = @Id", database._Connection);
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
                AveragePrice = SQLiteUtils.DBToDecimal(disposalReader.GetInt32(1)),
                TransactionCosts = SQLiteUtils.DBToDecimal(disposalReader.GetInt32(2)),
                CGTMethod = (CGTCalculationMethod)disposalReader.GetInt32(3),
                Comment = disposalReader.GetString(4)
            };
            disposalReader.Close();

            return disposal;
        }

        private static IncomeReceived CreateIncomeReceived(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get income received values */
            var command = new SQLiteCommand("SELECT [RecordDate], [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [Comment] FROM [IncomeReceived] WHERE [Id] = @Id", database._Connection);
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
                RecordDate = incomeReceivedReader.GetDateTime(0),
                FrankedAmount = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt32(1)),
                UnfrankedAmount = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt32(2)),
                FrankingCredits = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt32(3)),
                Interest = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt32(4)),
                TaxDeferred = SQLiteUtils.DBToDecimal(incomeReceivedReader.GetInt32(5)),
                Comment = incomeReceivedReader.GetString(6)
            };
            incomeReceivedReader.Close();

            return incomeReceived;
        }

        private static OpeningBalance CreateOpeningBalance(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Units], [CostBase], [AquisitionDate], [Comment] FROM [OpeningBalances] WHERE [Id] = @Id", database._Connection);
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
                CostBase = SQLiteUtils.DBToDecimal(openingBalanceReader.GetInt32(1)),
                AquisitionDate = openingBalanceReader.GetDateTime(2),
                Comment = openingBalanceReader.GetString(3)
            };  

            openingBalanceReader.Close();

            return openingBalance;
        }

        private static ReturnOfCapital CreateReturnOfCapital(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [RecordDate], [Amount], [Comment] FROM [ReturnsOfCapital] WHERE [Id] = @Id", database._Connection);
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
                RecordDate = returnOfCapitalReader.GetDateTime(0),
                Amount = SQLiteUtils.DBToDecimal(returnOfCapitalReader.GetInt32(1)),
                Comment = returnOfCapitalReader.GetString(2)
            };
            returnOfCapitalReader.Close();

            return returnOfCapital;
        }

        private static UnitCountAdjustment CreateUnitCountAdjustment(SQLitePortfolioDatabase database, Guid id)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [OriginalUnits], [NewUnits], [Comment] FROM [UnitCountAdjustments] WHERE [Id] = @Id", database._Connection);
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
                NewUnits = unitCountAdjustmentReader.GetInt32(1),
                Comment = unitCountAdjustmentReader.GetString(2)
            };
            
            unitCountAdjustmentReader.Close();

            return unitCountAdjustmnet;
        }
    }
}

