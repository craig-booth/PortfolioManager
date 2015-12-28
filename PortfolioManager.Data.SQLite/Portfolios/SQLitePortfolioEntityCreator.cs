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
        private static int DecimalToDB(decimal value)
        {
            return (int)Math.Floor(value * 100000);
        }

        private static decimal DBToDecimal(int value)
        {
            return (decimal)value / 100000;
        }

        public static ITransaction CreateTransaction(SQLitePortfolioDatabase database, SQLiteDataReader reader)
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
            var command = new SQLiteCommand("SELECT * FROM [Aquisitions] WHERE [Id] = @Id", database._Connection);
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
                Units = aquisitionReader.GetInt32(1),
                AveragePrice = DBToDecimal(aquisitionReader.GetInt32(2)),
                TransactionCosts = DBToDecimal(aquisitionReader.GetInt32(3)),
                Comment = aquisitionReader.GetString(4)
            };
            aquisitionReader.Close();

            return aquisition;
        }

        private static CostBaseAdjustment CreateCostBaseAdjustment(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get cost base adjsutment values */
            var command = new SQLiteCommand("SELECT * FROM [CostBaseAdjustments] WHERE [Id] = @Id", database._Connection);
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
                RecordDate = costBaseAdjustmentReader.GetDateTime(1),
                Percentage = DBToDecimal(costBaseAdjustmentReader.GetInt32(2)),
                Comment = costBaseAdjustmentReader.GetString(3)
            };
            costBaseAdjustmentReader.Close();

            return costBaseAdjustment;
        }

        private static Disposal CreateDisposal(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get disposal values */
            var command = new SQLiteCommand("SELECT * FROM [Disposals] WHERE [Id] = @Id", database._Connection);
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
                Units = disposalReader.GetInt32(1),
                AveragePrice = DBToDecimal(disposalReader.GetInt32(2)),
                TransactionCosts = DBToDecimal(disposalReader.GetInt32(3)),
                CGTMethod = (CGTCalculationMethod)disposalReader.GetInt32(4),
                Comment = disposalReader.GetString(5)
            };
            disposalReader.Close();

            return disposal;
        }

        private static IncomeReceived CreateIncomeReceived(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get income received values */
            var command = new SQLiteCommand("SELECT * FROM [IncomeReceived] WHERE [Id] = @Id", database._Connection);
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
                RecordDate = incomeReceivedReader.GetDateTime(1),
                FrankedAmount = DBToDecimal(incomeReceivedReader.GetInt32(2)),
                UnfrankedAmount = DBToDecimal(incomeReceivedReader.GetInt32(3)),
                FrankingCredits = DBToDecimal(incomeReceivedReader.GetInt32(4)),
                Interest = DBToDecimal(incomeReceivedReader.GetInt32(5)),
                TaxDeferred = DBToDecimal(incomeReceivedReader.GetInt32(6)),
                Comment = incomeReceivedReader.GetString(7)
            };
            incomeReceivedReader.Close();

            return incomeReceived;
        }

        private static OpeningBalance CreateOpeningBalance(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT * FROM [OpeningBalances] WHERE [Id] = @Id", database._Connection);
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
                Units = openingBalanceReader.GetInt32(1),
                CostBase = DBToDecimal(openingBalanceReader.GetInt32(2)),
                Comment = openingBalanceReader.GetString(3)
            };
            openingBalanceReader.Close();

            return openingBalance;
        }

        private static ReturnOfCapital CreateReturnOfCapital(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT * FROM [ReturnsOfCapital] WHERE [Id] = @Id", database._Connection);
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
                RecordDate = returnOfCapitalReader.GetDateTime(1),
                Amount = DBToDecimal(returnOfCapitalReader.GetInt32(2)),
                Comment = returnOfCapitalReader.GetString(3)
            };
            returnOfCapitalReader.Close();

            return returnOfCapital;
        }

        private static UnitCountAdjustment CreateUnitCountAdjustment(SQLitePortfolioDatabase database, SQLiteDataReader reader)
        {
            /* Get opening balance values */
            var command = new SQLiteCommand("SELECT * FROM [UnitCountAdjustments] WHERE [Id] = @Id", database._Connection);
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
                OriginalUnits = unitCountAdjustmentReader.GetInt32(1),
                NewUnits = unitCountAdjustmentReader.GetInt32(2),
                Comment = unitCountAdjustmentReader.GetString(3)
            };
            unitCountAdjustmentReader.Close();

            return unitCountAdjustmnet;
        }
    }
}

