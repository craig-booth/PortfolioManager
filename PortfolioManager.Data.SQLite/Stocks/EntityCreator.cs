using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockEntityCreator
    {
        public static Stock CreateStock(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            Stock stock = new Stock(database,
                                    new Guid(reader.GetString(0)),
                                    reader.GetDateTime(1),
                                    reader.GetDateTime(2),
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    (StockType)(reader.GetInt32(5)),
                                    new Guid(reader.GetString(6)));

            return stock;
        }

        public static RelativeNTA CreateRelativeNTA(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            RelativeNTA nta = new RelativeNTA(database,
                                              new Guid(reader.GetString(0)),
                                              reader.GetDateTime(1),
                                              new Guid(reader.GetString(2)),
                                              new Guid(reader.GetString(3)),
                                              DBToDecimal(reader.GetInt32(4)));
            return nta;
        }

        public static ICorporateAction CreateCorporateAction(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            int actionType = reader.GetInt32(4);

            if (actionType == 1)
                return CreateDividend(database, reader);
            else if (actionType == 2)
                return CreateCapitalReturn(database, reader);
            else if (actionType == 3)
                return CreateTransformation(database, reader);
            else
                return null;
        }

        private static Dividend CreateDividend(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            /* Get dividend vales */
            var command = new SQLiteCommand("SELECT * FROM [Dividends] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader dividendReader = command.ExecuteReader();
            if (!dividendReader.Read())
            {
                dividendReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            Dividend dividend = new Dividend(database,
                                    new Guid(reader.GetString(0)),
                                    new Guid(reader.GetString(1)),
                                    reader.GetDateTime(2),
                                    dividendReader.GetDateTime(1),
                                    DBToDecimal(dividendReader.GetInt32(2)),
                                    DBToDecimal(dividendReader.GetInt32(4)),
                                    DBToDecimal(dividendReader.GetInt32(3)),
                                    DBToDecimal(dividendReader.GetInt32(5)),
                                    reader.GetString(3));
            dividendReader.Close();

            return dividend;
        }

        private static CapitalReturn CreateCapitalReturn(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            /* Get capital return vales */
            var command = new SQLiteCommand("SELECT * FROM [CapitalReturns] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader capitalReturnReader = command.ExecuteReader();
            if (!capitalReturnReader.Read())
            {
                capitalReturnReader.Close();
                throw new RecordNotFoundException(reader.GetString(0));
            }

            CapitalReturn capitalReturn = new CapitalReturn(database,
                                    new Guid(reader.GetString(0)),
                                    new Guid(reader.GetString(1)),
                                    reader.GetDateTime(2),
                                    capitalReturnReader.GetDateTime(1),
                                    DBToDecimal(capitalReturnReader.GetInt32(2)),
                                    reader.GetString(3));
            capitalReturnReader.Close();

            return capitalReturn;
        }

        private static Transformation CreateTransformation(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            /* Get transformation vales */
            var command = new SQLiteCommand("SELECT * FROM [Transformations] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            SQLiteDataReader transformationReader = command.ExecuteReader();
            if (!transformationReader.Read())
            {
                throw new RecordNotFoundException(reader.GetString(0));
            }

            Transformation transformation = new Transformation(database,
                                    new Guid(reader.GetString(0)),
                                    new Guid(reader.GetString(1)),
                                    reader.GetDateTime(2),
                                    transformationReader.GetDateTime(1),
                                    DBToDecimal(transformationReader.GetInt32(2)),
                                    reader.GetString(3));
            transformationReader.Close();

            /* Get result stocks */
            command = new SQLiteCommand("SELECT * FROM [TransformationResultingStocks] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", reader.GetString(0));
            transformationReader = command.ExecuteReader();

            while (transformationReader.Read())
            {
                ResultingStock resultStock = new ResultingStock(new Guid(transformationReader.GetString(1)),
                                        transformationReader.GetInt32(2),
                                        transformationReader.GetInt32(3),
                                        DBToDecimal(transformationReader.GetInt32(4)));
                transformation.ResultingStocks.Add(resultStock);
            }

            transformationReader.Close();

            return transformation;
        }

        private static int DecimalToDB(decimal value)
        {
            return (int)Math.Floor(value * 100000);
        }

        private static decimal DBToDecimal(int value)
        {
            return (decimal)value / 100000;
        }

    }
}
