using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Data.SQLite;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockEntityCreator
    {
        public static Stock CreateStock(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            RoundingRule dividendRoundingRule = RoundingRule.Round;
            if (! reader.IsDBNull(7))
                dividendRoundingRule = (RoundingRule)reader.GetInt32(7);

            DRPMethod drpMethod = DRPMethod.Round;
            if (!reader.IsDBNull(8))
                drpMethod = (DRPMethod)reader.GetInt32(8);

            Stock stock = new Stock(new Guid(reader.GetString(0)),
                                    reader.GetDateTime(1),
                                    reader.GetDateTime(2),
                                    reader.GetString(3),
                                    reader.GetString(4),
                                    (StockType)(reader.GetInt32(5)),
                                    new Guid(reader.GetString(6)),
                                    dividendRoundingRule,
                                    drpMethod);

            return stock;
        }

        public static RelativeNTA CreateRelativeNTA(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            RelativeNTA nta = new RelativeNTA(new Guid(reader.GetString(0)),
                                              reader.GetDateTime(1),
                                              new Guid(reader.GetString(2)),
                                              new Guid(reader.GetString(3)),
                                              SQLiteUtils.DBToDecimal(reader.GetInt64(4)));
            return nta;
        }

        public static CorporateAction CreateCorporateAction(SQLiteStockDatabase database, SQLiteDataReader reader)
        {
            Guid id = new Guid(reader.GetString(0));
            Guid stock = new Guid(reader.GetString(1));
            DateTime actionDate = reader.GetDateTime(2);
            string description = reader.GetString(3);
            CorporateActionType type = (CorporateActionType)reader.GetInt32(4);

            return CreateCorporateAction(database, id, type, stock, actionDate, description);
        }

        public static CorporateAction CreateCorporateAction(SQLiteStockDatabase database, Guid id, CorporateActionType type, Guid stock, DateTime actionDate, string description)
        {
            if (type == CorporateActionType.Dividend)
                return CreateDividend(database, id, stock, actionDate, description);
            else if (type == CorporateActionType.CapitalReturn)
                return CreateCapitalReturn(database, id, stock, actionDate, description);
            else if (type == CorporateActionType.Transformation)
                return CreateTransformation(database, id, stock, actionDate, description);
            else if (type == CorporateActionType.SplitConsolidation)
                return CreateSplitConsolidation(database, id, stock, actionDate, description);
            else if (type == CorporateActionType.Composite)
                return CreateCompositeAction(database, id, stock, actionDate, description);
            else
                return null;
        }

        private static Dividend CreateDividend(SQLiteStockDatabase database, Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get dividend vales */
            var command = new SQLiteCommand("SELECT * FROM [Dividends] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader dividendReader = command.ExecuteReader();
            if (!dividendReader.Read())
            {
                dividendReader.Close();
                throw new RecordNotFoundException(id);
            }

            Dividend dividend = new Dividend(id,
                                    stock,
                                    actionDate,
                                    dividendReader.GetDateTime(1),
                                    SQLiteUtils.DBToDecimal(dividendReader.GetInt64(2)),
                                    SQLiteUtils.DBToDecimal(dividendReader.GetInt64(4)),
                                    SQLiteUtils.DBToDecimal(dividendReader.GetInt64(3)),
                                    SQLiteUtils.DBToDecimal(dividendReader.GetInt64(5)),
                                    description);
            dividendReader.Close();

            return dividend;
        }

        private static CapitalReturn CreateCapitalReturn(SQLiteStockDatabase database, Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get capital return vales */
            var command = new SQLiteCommand("SELECT * FROM [CapitalReturns] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader capitalReturnReader = command.ExecuteReader();
            if (!capitalReturnReader.Read())
            {
                capitalReturnReader.Close();
                throw new RecordNotFoundException(id);
            }

            CapitalReturn capitalReturn = new CapitalReturn(id,
                                    stock,
                                    actionDate,
                                    capitalReturnReader.GetDateTime(1),
                                    SQLiteUtils.DBToDecimal(capitalReturnReader.GetInt64(2)),
                                    description);
            capitalReturnReader.Close();

            return capitalReturn;
        }

        private static SplitConsolidation CreateSplitConsolidation(SQLiteStockDatabase database, Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get capital return vales */
            var command = new SQLiteCommand("SELECT * FROM [SplitConsolidations] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader spitConsolidationReader = command.ExecuteReader();
            if (!spitConsolidationReader.Read())
            {
                spitConsolidationReader.Close();
                throw new RecordNotFoundException(id);
            }

            var splitConsolidation = new SplitConsolidation(id,
                                    stock,
                                    actionDate,
                                    spitConsolidationReader.GetInt32(1),
                                    spitConsolidationReader.GetInt32(2),
                                    description);
            spitConsolidationReader.Close();

            return splitConsolidation;
        }

        private static Transformation CreateTransformation(SQLiteStockDatabase database, Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get transformation vales */
            var command = new SQLiteCommand("SELECT * FROM [Transformations] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader transformationReader = command.ExecuteReader();
            if (!transformationReader.Read())
            {
                throw new RecordNotFoundException(id);
            }

            Transformation transformation = new Transformation(id,
                                    stock,
                                    actionDate,
                                    transformationReader.GetDateTime(1),
                                    SQLiteUtils.DBToDecimal(transformationReader.GetInt64(2)),
                                    SQLiteUtils.DBToBool(transformationReader.GetString(3)),
                                    description);
            transformationReader.Close();

            /* Get result stocks */
            command = new SQLiteCommand("SELECT * FROM [TransformationResultingStocks] WHERE [Id] = @Id", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            transformationReader = command.ExecuteReader();

            while (transformationReader.Read())
            {
                ResultingStock resultingStock = new ResultingStock(new Guid(transformationReader.GetString(1)),
                                        transformationReader.GetInt32(2),
                                        transformationReader.GetInt32(3),
                                        SQLiteUtils.DBToDecimal(transformationReader.GetInt64(4)),
                                        transformationReader.GetDateTime(5));

                transformation.AddResultStock(resultingStock);
            }

            transformationReader.Close();

            return transformation;
        }


        private static CompositeAction CreateCompositeAction(SQLiteStockDatabase database, Guid id, Guid stock, DateTime actionDate, string description)
        {
            CompositeAction compositeAction = new CompositeAction(id,
                        stock,
                        actionDate,
                        description);

            /* Get composite action children */
            var command = new SQLiteCommand("SELECT [ChildAction], [ChildType] FROM [CompositeActions] WHERE [Id] = @Id ORDER BY [Sequence]", database._Connection);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());
            SQLiteDataReader compositeActionReader = command.ExecuteReader();
            while (compositeActionReader.Read())
            {
                var childId = new Guid(compositeActionReader.GetString(0));
                var childType = (CorporateActionType)compositeActionReader.GetInt32(1);

                var childAction = CreateCorporateAction(database, childId, childType, stock, actionDate, description);

                compositeAction.Children.Add(childAction);
            }


            compositeActionReader.Close();

            return compositeAction ;
        }

    }
}
