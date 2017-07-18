using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Data.SQLite.Stocks
{
    public class SQLiteStockEntityCreator : IEntityCreator
    {
        private readonly Dictionary<Type, Func<SqliteDataReader, Entity>> _EntityCreators;

        public SQLiteStockEntityCreator()
        {
            _EntityCreators = new Dictionary<Type, Func<SqliteDataReader, Entity>>();

            _EntityCreators.Add(typeof(Stock), CreateStock);
            _EntityCreators.Add(typeof(RelativeNTA), CreateRelativeNTA);

            _EntityCreators.Add(typeof(CapitalReturn), CreateCapitalReturn);
            _EntityCreators.Add(typeof(Dividend), CreateDividend);
            _EntityCreators.Add(typeof(CompositeAction), CreateCompositeAction);
            _EntityCreators.Add(typeof(SplitConsolidation), CreateSplitConsolidation);
            _EntityCreators.Add(typeof(Transformation), CreateTransformation);
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

        public Dividend CreateDividend(SqliteDataReader reader)
        {
            var dividend = new Dividend(new Guid(reader.GetString(0)),
                                        new Guid(reader.GetString(1)),
                                        reader.GetDateTime(2),
                                        reader.GetDateTime(6),
                                        SQLiteUtils.DBToDecimal(reader.GetInt64(7)),
                                        SQLiteUtils.DBToDecimal(reader.GetInt64(8)),
                                        SQLiteUtils.DBToDecimal(reader.GetInt64(9)),
                                        SQLiteUtils.DBToDecimal(reader.GetInt64(10)),
                                        reader.GetString(3));
            return dividend;
        }

        private CapitalReturn CreateCapitalReturn(SqliteDataReader reader)
        {
            var capitalReturn = new CapitalReturn(new Guid(reader.GetString(0)),
                                        new Guid(reader.GetString(1)),
                                        reader.GetDateTime(2),
                                        reader.GetDateTime(6),
                                        SQLiteUtils.DBToDecimal(reader.GetInt64(7)),
                                        reader.GetString(3));

            return capitalReturn;
        }

        private SplitConsolidation CreateSplitConsolidation(SqliteDataReader reader)
        {
            var splitConsolidation = new SplitConsolidation(new Guid(reader.GetString(0)),
                                        new Guid(reader.GetString(1)),
                                        reader.GetDateTime(2),
                                        reader.GetInt32(6),
                                        reader.GetInt32(7),
                                        reader.GetString(3));

            return splitConsolidation;
        }

        private Transformation CreateTransformation(SqliteDataReader reader)
        {
            var transformation = new Transformation(new Guid(reader.GetString(0)),
                                        new Guid(reader.GetString(1)),
                                        reader.GetDateTime(2),
                                        reader.GetDateTime(6),
                                        SQLiteUtils.DBToDecimal(reader.GetInt64(7)),
                                        SQLiteUtils.DBToBool(reader.GetString(8)),
                                        reader.GetString(3));

            var resultingStock = new ResultingStock(new Guid(reader.GetString(10)),
                                                reader.GetInt32(11),
                                                reader.GetInt32(12),
                                                SQLiteUtils.DBToDecimal(reader.GetInt64(13)),
                                                reader.GetDateTime(14));

            transformation.AddResultStock(resultingStock);

            while (reader.Read())
            {
                resultingStock = new ResultingStock(new Guid(reader.GetString(10)),
                                            reader.GetInt32(11),
                                            reader.GetInt32(12),
                                            SQLiteUtils.DBToDecimal(reader.GetInt64(13)),
                                            reader.GetDateTime(14));

                transformation.AddResultStock(resultingStock);
            }

            return transformation;
        }


        private CompositeAction CreateCompositeAction(SqliteDataReader reader)
        {
            var compositeAction = new CompositeAction(new Guid(reader.GetString(0)),
                                        new Guid(reader.GetString(1)),
                                        reader.GetDateTime(2),
                                        reader.GetString(3));

            return compositeAction;
        }

    }
}
