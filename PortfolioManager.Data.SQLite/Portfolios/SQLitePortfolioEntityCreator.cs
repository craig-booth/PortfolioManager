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
            TransactionType type = (TransactionType)reader.GetInt32(3);

            if (type == TransactionType.Aquisition)
                return CreateAquisition(database, reader);
            else if (type == TransactionType.CostBaseAdjustment)
                return CreateCostBaseAdjustment(database, reader);
            else if (type == TransactionType.Disposal)
                return CreateDisposal(database, reader);
            else if (type == TransactionType.Income)
                return CreateIncomeReceived(database, reader);
            else if (type == TransactionType.OpeningBalance)
                return CreateOpeningBalance(database, reader);
            else if (type == TransactionType.ReturnOfCapital)
                return CreateReturnOfCapital(database, reader);
            else if (type == TransactionType.UnitCountAdjustment)
                return CreateUnitCountAdjustment(database, reader);
            else
                return null;
        }

        private static Aquisition CreateAquisition(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get aquisition values */
            var command = new SQLiteCommand("SELECT [Units],[AveragePrice], [TransactionCosts], [Comment] FROM [Aquisitions] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader aquisitionReader = command.ExecuteReader();
            if (!aquisitionReader.Read())
            {
                aquisitionReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            Aquisition aquisition = new Aquisition(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Units = aquisitionReader.GetInt32(0),
                AveragePrice = SQLiteUtils.DBToDecimal(aquisitionReader.GetInt32(1)),
                TransactionCosts = SQLiteUtils.DBToDecimal(aquisitionReader.GetInt32(2)),
                Comment = aquisitionReader.GetString(3)
            };
            aquisitionReader.Close();

            return aquisition;
        }

        private static CostBaseAdjustment CreateCostBaseAdjustment(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get cost base adjsutment values */
            var command = new SQLiteCommand("SELECT [RecordDate], [Percentage], [Comment] FROM [CostBaseAdjustments] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader costBaseAdjustmentReader = command.ExecuteReader();
            if (!costBaseAdjustmentReader.Read())
            {
                costBaseAdjustmentReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            CostBaseAdjustment costBaseAdjustment = new CostBaseAdjustment(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                RecordDate = costBaseAdjustmentReader.GetDateTime(0),
                Percentage = SQLiteUtils.DBToDecimal(costBaseAdjustmentReader.GetInt32(1)),
                Comment = costBaseAdjustmentReader.GetString(2)
            };
            costBaseAdjustmentReader.Close();

            return costBaseAdjustment;
        }

        private static Disposal CreateDisposal(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get disposal values */
            var command = new SQLiteCommand("SELECT [Units], [AveragePrice], [TransactionCosts], [CGTMethod], [Comment] FROM [Disposals] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader disposalReader = command.ExecuteReader();
            if (!disposalReader.Read())
            {
                disposalReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            Disposal disposal = new Disposal(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Units = disposalReader.GetInt32(0),
                AveragePrice = SQLiteUtils.DBToDecimal(disposalReader.GetInt32(1)),
                TransactionCosts = SQLiteUtils.DBToDecimal(disposalReader.GetInt32(2)),
                CGTMethod = (CGTCalculationMethod)disposalReader.GetInt32(3),
                Comment = disposalReader.GetString(4)
            };
            disposalReader.Close();

            return disposal;
        }

        private static IncomeReceived CreateIncomeReceived(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get income received values */
            var command = new SQLiteCommand("SELECT [RecordDate], [FrankedAmount], [UnfrankedAmount], [FrankingCredits], [Interest], [TaxDeferred], [Comment] FROM [IncomeReceived] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader incomeReceivedReader = command.ExecuteReader();
            if (!incomeReceivedReader.Read())
            {
                incomeReceivedReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            IncomeReceived incomeReceived = new IncomeReceived(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
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

        private static OpeningBalance CreateOpeningBalance(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [Units], [CostBase], [AquisitionDate], [Comment] FROM [OpeningBalances] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader openingBalanceReader = command.ExecuteReader();
            if (!openingBalanceReader.Read())
            {
                openingBalanceReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            OpeningBalance openingBalance = new OpeningBalance(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                Units = openingBalanceReader.GetInt32(0),
                CostBase = SQLiteUtils.DBToDecimal(openingBalanceReader.GetInt32(1)),
                AquisitionDate = openingBalanceReader.GetDateTime(2),
                Comment = openingBalanceReader.GetString(3)
            };  

            openingBalanceReader.Close();

            return openingBalance;
        }

        private static ReturnOfCapital CreateReturnOfCapital(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [RecordDate], [Amount], [Comment] FROM [ReturnsOfCapital] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader returnOfCapitalReader = command.ExecuteReader();
            if (!returnOfCapitalReader.Read())
            {
                returnOfCapitalReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            ReturnOfCapital returnOfCapital = new ReturnOfCapital(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                RecordDate = returnOfCapitalReader.GetDateTime(0),
                Amount = SQLiteUtils.DBToDecimal(returnOfCapitalReader.GetInt32(1)),
                Comment = returnOfCapitalReader.GetString(2)
            };
            returnOfCapitalReader.Close();

            return returnOfCapital;
        }

        private static UnitCountAdjustment CreateUnitCountAdjustment(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT [OriginalUnits], [NewUnits], [Comment] FROM [UnitCountAdjustments] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader unitCountAdjustmentReader = command.ExecuteReader();
            if (!unitCountAdjustmentReader.Read())
            {
                unitCountAdjustmentReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            UnitCountAdjustment unitCountAdjustmnet = new UnitCountAdjustment(new Guid(reader.GetString(0)))
            {
                TransactionDate = reader.GetDateTime(1),
                ASXCode = reader.GetString(4),
                OriginalUnits = unitCountAdjustmentReader.GetInt32(0),
                NewUnits = unitCountAdjustmentReader.GetInt32(1),
                Comment = unitCountAdjustmentReader.GetString(2)
            };
            
            unitCountAdjustmentReader.Close();

            return unitCountAdjustmnet;
        }
    }
}

