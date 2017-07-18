using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using PortfolioManager.Data.Stocks;

using PortfolioManager.Common;

namespace PortfolioManager.Data.SQLite.Stocks.CorporateActions
{
    class SQLiteCorporateActionQuery: SQLiteQuery, ICorporateActionQuery
    {

        protected internal SQLiteCorporateActionQuery(SqliteTransaction transaction)
            : base(transaction, new SQLiteStockEntityCreator())
        {
        }

        public CorporateAction Get(Guid id)
        {
            var query = EntityQuery.FromTable("CorporateActions")
                .Select("[Id], [Type]")
                .WithId(id);

            using (var reader = query.Execute())
            {
                if (reader.Read())
                    return Get(new Guid(reader.GetString(0)), (CorporateActionType)reader.GetInt32(1));
                else
                    return null;
            } 
        }

        private T Get<T>(Guid id) where T : CorporateAction
        {
            var query = EntityQuery.FromTable("CorporateActions")
                            .WithId(id);

            if (typeof(T) == typeof(CapitalReturn))
                query.Join("CapitalReturns", "CorporateActions.Id = CapitalReturns.Id");
            else if (typeof(T) == typeof(CompositeAction))
            {
                query.Join("CompositeActions", "CorporateActions.Id = CompositeActions.Id");        
            }
            else if (typeof(T) == typeof(Dividend))
                query.Join("Dividends", "CorporateActions.Id = Dividends.Id");
            else if (typeof(T) == typeof(SplitConsolidation))
                query.Join("SplitConsolidations", "CorporateActions.Id = SplitConsolidations.Id");
            else if (typeof(T) == typeof(Transformation))
            {
                query.Join("Transformations", "CorporateActions.Id = Transformations.Id")
                    .Join("TransformationResultingStocks", "Transformations.Id = TransformationResultingStocks.Id");
            }

            return query.CreateEntity<T>(); 
        }

        private CorporateAction Get(Guid id, CorporateActionType type)
        {
            if (type == CorporateActionType.CapitalReturn)
                return Get<CapitalReturn>(id);
            else if (type == CorporateActionType.Composite)
                return Get<CompositeAction>(id);
            else if (type == CorporateActionType.Dividend)
                return Get<Dividend>(id);
            else if (type == CorporateActionType.SplitConsolidation)
                return Get<SplitConsolidation>(id);
            if (type == CorporateActionType.Transformation)
                return Get<Transformation>(id);
            else
                return null;
        }
    
   
        public IEnumerable<CorporateAction> Find(Guid stock, DateTime fromDate, DateTime toDate)
        {
            var query = EntityQuery.FromTable("CorporateActions")
                                    .Where("[Stock] = @Stock AND [ActionDate] BETWEEN @FromDate AND @ToDate")
                                    .WithParameter("@Stock", stock)
                                    .WithParameter("@FromDate", fromDate)
                                    .WithParameter("@ToDate", toDate);

            var actions = new List<CorporateAction>();
            using (var reader = query.Execute())
            {
                while (reader.Read())
                {
                    actions.Add(Get(new Guid(reader.GetString(0)), (CorporateActionType)reader.GetInt32(1)));
                }
            }

            return actions;
        }
    }
}
