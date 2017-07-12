using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockEntityCreator : IEntityCreator
    {
        private readonly SqliteTransaction _Transaction;
        private readonly Dictionary<Type, Func<SqliteDataReader, Entity>> _EntityCreators;

        public SQLiteStockEntityCreator(SqliteTransaction transaction)
        {
            _Transaction = transaction;
            _EntityCreators = new Dictionary<Type, Func<SqliteDataReader, Entity>>();

            _EntityCreators.Add(typeof(Stock), CreateStock);
            _EntityCreators.Add(typeof(RelativeNTA), CreateRelativeNTA);
            _EntityCreators.Add(typeof(CorporateAction), CreateCorporateAction);
        }

        public T CreateEntity<T>(SqliteDataReader reader) where T: Entity
        {
            Func<SqliteDataReader, Entity> creationFunction;

            if (_EntityCreators.TryGetValue(typeof(T), out creationFunction))
                return (T)creationFunction(reader);
            else
                return default(T);
        }

        private Stock CreateStock(SqliteDataReader reader)
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
                                    drpMethod,
                                    (AssetCategory)reader.GetInt32(9));

            return stock;
        }

        private RelativeNTA CreateRelativeNTA(SqliteDataReader reader)
        {
            RelativeNTA nta = new RelativeNTA(new Guid(reader.GetString(0)),
                                              reader.GetDateTime(1),
                                              new Guid(reader.GetString(2)),
                                              new Guid(reader.GetString(3)),
                                              SQLiteUtils.DBToDecimal(reader.GetInt64(4)));
            return nta;
        }

        private CorporateAction CreateCorporateAction(SqliteDataReader reader)
        {
            Guid id = new Guid(reader.GetString(0));
            Guid stock = new Guid(reader.GetString(1));
            DateTime actionDate = reader.GetDateTime(2);
            string description = reader.GetString(3);
            CorporateActionType type = (CorporateActionType)reader.GetInt32(4);

            return CreateCorporateAction(id, type, stock, actionDate, description);
        }

        private CorporateAction CreateCorporateAction(Guid id, CorporateActionType type, Guid stock, DateTime actionDate, string description)
        {
            if (type == CorporateActionType.Dividend)
                return CreateDividend(id, stock, actionDate, description);
            else if (type == CorporateActionType.CapitalReturn)
                return CreateCapitalReturn(id, stock, actionDate, description);
            else if (type == CorporateActionType.Transformation)
                return CreateTransformation(id, stock, actionDate, description);
            else if (type == CorporateActionType.SplitConsolidation)
                return CreateSplitConsolidation(id, stock, actionDate, description);
            else if (type == CorporateActionType.Composite)
                return CreateCompositeAction(id, stock, actionDate, description);
            else
                return null;
        }

        private Dividend CreateDividend(Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get dividend vales */
            var command = new SqliteCommand("SELECT * FROM [Dividends] WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());

            Dividend dividend;

            using (SqliteDataReader dividendReader = command.ExecuteReader())
            {
                if (!dividendReader.Read())
                {
                    throw new RecordNotFoundException(id);
                }

                dividend = new Dividend(id,
                                        stock,
                                        actionDate,
                                        dividendReader.GetDateTime(1),
                                        SQLiteUtils.DBToDecimal(dividendReader.GetInt64(2)),
                                        SQLiteUtils.DBToDecimal(dividendReader.GetInt64(4)),
                                        SQLiteUtils.DBToDecimal(dividendReader.GetInt64(3)),
                                        SQLiteUtils.DBToDecimal(dividendReader.GetInt64(5)),
                                        description);
            }


            return dividend;
        }

        private CapitalReturn CreateCapitalReturn(Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get capital return vales */
            var command = new SqliteCommand("SELECT * FROM [CapitalReturns] WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());

            CapitalReturn capitalReturn;
            using (SqliteDataReader capitalReturnReader = command.ExecuteReader())
            {
                if (!capitalReturnReader.Read())
                {
                    throw new RecordNotFoundException(id);
                }

                capitalReturn = new CapitalReturn(id,
                                        stock,
                                        actionDate,
                                        capitalReturnReader.GetDateTime(1),
                                        SQLiteUtils.DBToDecimal(capitalReturnReader.GetInt64(2)),
                                        description);
            }


            return capitalReturn;
        }

        private SplitConsolidation CreateSplitConsolidation(Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get capital return vales */
            var command = new SqliteCommand("SELECT * FROM [SplitConsolidations] WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());

            SplitConsolidation splitConsolidation;
            using (SqliteDataReader spitConsolidationReader = command.ExecuteReader())
            {
                if (!spitConsolidationReader.Read())
                {
                    throw new RecordNotFoundException(id);
                }

                splitConsolidation = new SplitConsolidation(id,
                                        stock,
                                        actionDate,
                                        spitConsolidationReader.GetInt32(1),
                                        spitConsolidationReader.GetInt32(2),
                                        description);
            }
   

            return splitConsolidation;
        }

        private Transformation CreateTransformation(Guid id, Guid stock, DateTime actionDate, string description)
        {
            /* Get transformation vales */
            var command = new SqliteCommand("SELECT * FROM [Transformations] WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());

            Transformation transformation;
            using (SqliteDataReader transformationReader = command.ExecuteReader())
            {
                if (!transformationReader.Read())
                {
                    throw new RecordNotFoundException(id);
                }

                transformation = new Transformation(id,
                                        stock,
                                        actionDate,
                                        transformationReader.GetDateTime(1),
                                        SQLiteUtils.DBToDecimal(transformationReader.GetInt64(2)),
                                        SQLiteUtils.DBToBool(transformationReader.GetString(3)),
                                        description);
            }


            /* Get result stocks */
            command = new SqliteCommand("SELECT * FROM [TransformationResultingStocks] WHERE [Id] = @Id", _Transaction.Connection, _Transaction);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());

            using (var transformationReader = command.ExecuteReader())
            {
                while (transformationReader.Read())
                {
                    ResultingStock resultingStock = new ResultingStock(new Guid(transformationReader.GetString(1)),
                                            transformationReader.GetInt32(2),
                                            transformationReader.GetInt32(3),
                                            SQLiteUtils.DBToDecimal(transformationReader.GetInt64(4)),
                                            transformationReader.GetDateTime(5));

                    transformation.AddResultStock(resultingStock);
                }
            }

            return transformation;
        }


        private CompositeAction CreateCompositeAction(Guid id, Guid stock, DateTime actionDate, string description)
        {
            CompositeAction compositeAction = new CompositeAction(id,
                        stock,
                        actionDate,
                        description);

            /* Get composite action children */
            var command = new SqliteCommand("SELECT [ChildAction], [ChildType] FROM [CompositeActions] WHERE [Id] = @Id ORDER BY [Sequence]", _Transaction.Connection, _Transaction);
            command.Prepare();
            command.Parameters.AddWithValue("@Id", id.ToString());

            using (SqliteDataReader compositeActionReader = command.ExecuteReader())
            {
                while (compositeActionReader.Read())
                {
                    var childId = new Guid(compositeActionReader.GetString(0));
                    var childType = (CorporateActionType)compositeActionReader.GetInt32(1);

                    var childAction = CreateCorporateAction(childId, childType, stock, actionDate, description);

                    compositeAction.Children.Add(childAction);
                }
            }

            return compositeAction ;
        }

    }
}
