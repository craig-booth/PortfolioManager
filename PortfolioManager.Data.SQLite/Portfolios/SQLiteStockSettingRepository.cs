using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PortfolioManager.Common;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Portfolios;

namespace PortfolioManager.Data.SQLite.Portfolios
{
    class SQLiteStockSettingRepository : IStockSettingRepository
    {
        private SQLitePortfolioDatabase _Database;

        protected internal SQLiteStockSettingRepository(SQLitePortfolioDatabase database)
        {
            _Database = database;
        }

        public StockSetting Get(Guid id)
        {
            return Get(id, DateTime.Today);
        }

        public StockSetting Get(Guid id, DateTime atDate)
        {
            var stockSettingQuery = from stockSetting in _Database._StockSettings
                                  where (stockSetting.Id == id) && ((atDate >= stockSetting.FromDate && atDate <= stockSetting.ToDate))
                                  select stockSetting;

            return stockSettingQuery.FirstOrDefault();
        }

        public void Add(StockSetting entity)
        {
            _Database._StockSettings.Add(entity);
        }

        public void Update(StockSetting entity)
        {

        }

        public void Delete(StockSetting entity)
        {
            _Database._StockSettings.Remove(entity);
        }

        public void Delete(Guid id)
        {

        }

        public void Delete(Guid id, DateTime atDate)
        {
        }
    }
}
