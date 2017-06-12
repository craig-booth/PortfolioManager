﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;

namespace PortfolioManager.Data.SQLite.Stocks
{
    class SQLiteStockUnitOfWork : IStockUnitOfWork
    {
        private SQLiteStockDatabase _Database;
        private SQLiteStockEntityCreator _EntityCreator;

        SQLiteStockRepository _StockRepository;
        SQLiteCorporateActionRepository _CorporateActionRepository;
        SQLiteRelateNTARepository _RelativeNTARepository;
        SQLiteStockPriceRepository _StockPriceRepository;
        SQLiteNonTradingDayRepository _NonTradingDayRepository;

        public IStockRepository StockRepository
        {
            get
            {
                if (_StockRepository == null)
                    _StockRepository = new SQLiteStockRepository(_Database, _EntityCreator);

                return _StockRepository;
            }
        }

        public ICorporateActionRepository CorporateActionRepository
        {
            get
            {
                if (_CorporateActionRepository == null)
                    _CorporateActionRepository = new SQLiteCorporateActionRepository(_Database, _EntityCreator);

                return _CorporateActionRepository;
            }
        }

        public IRelativeNTARepository RelativeNTARepository
        {
            get
            {
                if (_RelativeNTARepository == null)
                    _RelativeNTARepository = new SQLiteRelateNTARepository(_Database, _EntityCreator);

                return _RelativeNTARepository;
            }
        }

        public IStockPriceRepository StockPriceRepository
        {
            get
            {
                if (_StockPriceRepository == null)
                    _StockPriceRepository = new SQLiteStockPriceRepository(_Database);

                return _StockPriceRepository;
            }
        }

        public INonTradingDayRepository NonTradingDayRepository
        {
            get
            {
                if (_NonTradingDayRepository == null)
                    _NonTradingDayRepository = new SQLiteNonTradingDayRepository(_Database);

                return _NonTradingDayRepository;
            }
        }

        protected internal SQLiteStockUnitOfWork(SQLiteStockDatabase database)
        {
            _Database = database;
            _Database._Transaction.BeginTransaction();
            _EntityCreator = new SQLiteStockEntityCreator(database);
        }

        public void Save()
        {
            _Database._Transaction.SaveOnEnd = true;
        }

        public void Dispose()
        {
            _Database._Transaction.EndTransaction();
        }
    }


}
