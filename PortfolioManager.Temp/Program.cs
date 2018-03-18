﻿using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Stocks.Commands;
using PortfolioManager.EventStore;
using PortfolioManager.Service.Services;
using PortfolioManager.Data.SQLite.Stocks;

namespace PortfolioManager.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventStore = new SqliteEventStore(@"C:\PortfolioManager\Events.db");
            var stockExchange = new StockExchange(eventStore);

            stockExchange.Load();

            //  Load(stockExchange, eventStore);
            //  LoadCorporateActions(stockExchange, eventStore);

            //  var stocks = stockExchange.Stocks.All(DateTime.Today).OrderBy(x => x.Properties[DateTime.Today].ASXCode);
            //  WritePrices(stocks);
            //   Console.ReadKey();

            TestCorporateActionPerformance(stockExchange);
        }

        private static async void TestCorporateActionPerformance(StockExchange stockExchange)
        {
            var fromDate = new DateTime(2016, 07, 01);
            var toDate = new DateTime(2017, 06, 30);

            for (var i = 0; i < 100; i++)
            {
                var service = new StockService(stockExchange);

                var stocks = stockExchange.Stocks.All().Where(x => x.IsEffectiveDuring(new DateRange(fromDate, toDate)));
                foreach (var stock in stocks)
                {
                    var responce = await service.GetCorporateActions(stock.Id, fromDate, toDate);
                }
            }
        }

        private static void WritePrices(IEnumerable<Stock> stocks)
        {
            foreach (var stock in stocks)
            {
                var properties = stock.Properties[DateTime.Today];
                Console.WriteLine("{0} ({1}): {2}", properties.ASXCode, properties.Name, stock.GetPrice(DateTime.Today));
                
                if (stock is StapledSecurity)
                {
                    var stapledSecurity = stock as StapledSecurity;
                    foreach (var childSecurity in stapledSecurity.ChildSecurities)
                    {
                        Console.WriteLine("    {0} ({1})", childSecurity.ASXCode, childSecurity.Name);
                    }
                }

            }
        }

        private static void LoadCorporateActions(StockExchange stockExchange, IEventStore eventStore)
        {
            var stockDataBase = new SQLiteStockDatabase(@"C:\PortfolioManager\Stocks.db");

            var commands = new List<ICommand>();

            using (var unitOfWork = stockDataBase.CreateReadOnlyUnitOfWork())
            {
                foreach (var stock in stockExchange.Stocks.All())
                {
                    var oldStock = unitOfWork.StockQuery.GetByASXCode(stock.Properties[stock.EffectivePeriod.FromDate].ASXCode, stock.EffectivePeriod.FromDate);

                    var corporateActions = unitOfWork.CorporateActionQuery.Find(oldStock.Id, DateUtils.NoStartDate, DateUtils.NoEndDate);
                    foreach (var corporateAction in corporateActions)
                    {
                        if (corporateAction.Type == CorporateActionType.CapitalReturn)
                        {
                            var capitalReturn = corporateAction as PortfolioManager.Data.Stocks.CapitalReturn;
                            commands.Add(new AddCapitalReturnCommand(stock.Properties[capitalReturn.ActionDate].ASXCode, capitalReturn.ActionDate, capitalReturn.Description, capitalReturn.PaymentDate, capitalReturn.Amount));
                        }
                        else if (corporateAction.Type == CorporateActionType.Dividend)
                        {
                            var dividend = corporateAction as PortfolioManager.Data.Stocks.Dividend;
                            commands.Add(new AddDividendCommand(stock.Properties[dividend.ActionDate].ASXCode, dividend.ActionDate, dividend.Description, dividend.PaymentDate, dividend.DividendAmount, dividend.CompanyTaxRate, dividend.PercentFranked, dividend.DRPPrice));
                        }
                    }
                }
                
            }

            var commandHandler = new StockExchangeCommandHandler(stockExchange);
            foreach (var command in commands)
            {
                dynamic c = command;
                commandHandler.Execute(c);
            } 
        }

        private static void Load(StockExchange stockExchange, IEventStore eventStore)
        {
            var stockDataBase = new SQLiteStockDatabase(@"C:\PortfolioManager\Stocks.db");

            var commands = new List<ICommand>();

            using (var unitOfWork = stockDataBase.CreateReadOnlyUnitOfWork())
            {
                var nonTradingDays = unitOfWork.StockQuery.NonTradingDays();
                commands.AddRange(nonTradingDays.Select(x => new AddNonTradingDayCommand(x)));

                commands.AddRange(LoadStocks(unitOfWork.StockQuery));
            }

            var commandHandler = new StockExchangeCommandHandler(stockExchange);
            foreach (var command in commands)
            {
                dynamic c = command;
                commandHandler.Execute(c);
            }

        }

        private static IEnumerable<ICommand> LoadStocks(Data.Stocks.IStockQuery stockQuery)
        {
            var commands = new List<ICommand>();

            var stocks = stockQuery.GetAll().Where(x => x.Type != StockType.StapledSecurity && x.ParentId == Guid.Empty).Select(x => x.Id).Distinct();
            foreach (var stock in stocks)
                commands.AddRange(CommandsForStock(stock, stockQuery));

            stocks = stockQuery.GetAll().Where(x => x.Type == StockType.StapledSecurity).OrderBy(x => x.FromDate).Select(x => x.Id).Distinct().ToList();
            foreach (var stock in stocks)
            {
                commands.AddRange(CommandsForStock(stock, stockQuery));
                commands.AddRange(LoadRelativeNTAs(stock, stockQuery));
            }

            stocks = stockQuery.GetAll().Where(x => x.ParentId == Guid.Empty).Select(x => x.Id).Distinct();
            foreach (var stock in stocks) 
                commands.AddRange(LoadPrices(stock, stockQuery));

            return commands;
        }

        private static IEnumerable<ICommand> CommandsForStock(Guid id, Data.Stocks.IStockQuery stockQuery)
        {
            var commands = new List<ICommand>();

            var stockVersions = stockQuery.GetAll().Where(x => x.Id == id).OrderBy(x => x.FromDate);

            var firstVersion = stockVersions.First();
            if (firstVersion.Type == StockType.StapledSecurity)
            {
                var childSecurities = stockQuery.GetChildStocks(id, firstVersion.FromDate).Select(x => new ListStapledSecurityCommand.StapledSecurityChild(x.ASXCode, x.Name, (x.Type == StockType.Trust)));
                commands.Add(new ListStapledSecurityCommand(firstVersion.ASXCode, firstVersion.Name, firstVersion.FromDate, firstVersion.Category, childSecurities));
            }
            else
            {
                var trust = (firstVersion.Type == StockType.Trust);
                commands.Add(new ListStockCommand(firstVersion.ASXCode, firstVersion.Name, firstVersion.FromDate, trust, firstVersion.Category));
            }

            foreach (var otherVersion in stockVersions.Skip(1))
            {
                commands.Add(new ChangeStockCommand(otherVersion.ASXCode, otherVersion.FromDate, otherVersion.ASXCode, otherVersion.Name, otherVersion.Category));
            }

            var lastVersion = stockVersions.Last();
            if (lastVersion.ToDate != DateUtils.NoEndDate)
            {
                commands.Add(new DelistStockCommand(lastVersion.ASXCode, lastVersion.ToDate));
            }

            return commands;
        }

        private static IEnumerable<ICommand> LoadPrices(Guid id, Data.Stocks.IStockQuery stockQuery)
        {
            var commands = new List<ICommand>();

            Data.Stocks.Stock stock = null;

            var prices = stockQuery.GetPrices(id, DateUtils.NoStartDate, DateTime.Today.AddDays(-1));
            foreach(var price in prices)
            {
                try
                {
                    if ((stock == null) || ! stock.IsEffectiveAt(price.Key))
                        stock = stockQuery.Get(id, price.Key);

                    var asxCode = stock.ASXCode;

                    commands.Add(new AddClosingPriceCommand(asxCode, price.Key, price.Value));
                }
                catch
                {

                }
            } 


            return commands;
        }

        private static IEnumerable<ICommand> LoadRelativeNTAs(Guid id, Data.Stocks.IStockQuery stockQuery)
        {
            var commands = new List<ICommand>();

            var stockVersions = stockQuery.GetAll().Where(x => x.Id == id).OrderBy(x => x.FromDate);
            var firstVersion = stockVersions.First();

            var childStocks = stockQuery.GetChildStocks(id, firstVersion.FromDate);
            var dates = stockQuery.GetRelativeNTAs(id, childStocks.First().Id).Select(x => x.Date);

            var relativeNTAS = new decimal[childStocks.Count()];
            int i;
            foreach (var date in dates)
            {
                i = 0;
                foreach (var childStock in childStocks)
                    relativeNTAS[i++] = stockQuery.GetRelativeNTA(id, childStock.Id, date).Percentage;

                commands.Add(new ChangeRelativeNTACommand(firstVersion.ASXCode, date, relativeNTAS));

            }


            return commands;
        }
    }

    public class StockExchangeCommandHandler :
        ICommandHandler<ListStockCommand>,
        ICommandHandler<ListStapledSecurityCommand>,
        ICommandHandler<DelistStockCommand>,
        ICommandHandler<AddClosingPriceCommand>,
        ICommandHandler<AddNonTradingDayCommand>,
        ICommandHandler<UpdateCurrentPriceCommand>,
        ICommandHandler<ChangeRelativeNTACommand>,
        ICommandHandler<ChangeStockCommand>,
        ICommandHandler<AddCapitalReturnCommand>,
        ICommandHandler<AddDividendCommand>
    {
        private StockExchange _StockExchange;

        public StockExchangeCommandHandler(StockExchange stockExchange)
        {
            _StockExchange = stockExchange;
        }

        public void Execute(ListStockCommand command)
        {
            _StockExchange.Stocks.ListStock(command.ASXCode, command.Name, command.ListingDate, command.Trust, command.Category);
        }

        public void Execute(ListStapledSecurityCommand command)
        {
            var childSecurities = command.ChildSecurities.Select(x => new StapledSecurityChild(x.ASXCode, x.Name, x.Trust));
            _StockExchange.Stocks.ListStapledSecurity(command.ASXCode, command.Name, command.ListingDate,command.Category, childSecurities);
        }

        public void Execute(ChangeStockCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.ChangeDate);
            if (stock != null)
                stock.ChangeProperties(command.ChangeDate, command.NewASXCode, command.Name, command.Category);
        }

        public void Execute(ChangeDividendReinvestmentPlanCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.ChangeDate);
            if (stock != null)
                stock.ChangeDRPRules(command.ChangeDate, command.DRPActive, command.RoundingRule, command.Method);
        }

        public void Execute(DelistStockCommand command)
        {
            _StockExchange.Stocks.DelistStock(command.ASXCode, command.Date);
        }

        public void Execute(AddClosingPriceCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.Date);
            if (stock != null)
                stock.UpdateClosingPrice(command.Date, command.ClosingPrice);
        }

        public void Execute(UpdateCurrentPriceCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, DateTime.Today);
            if (stock != null)
                stock.UpdateCurrentPrice(command.CurrentPrice);
        }


        public void Execute(ChangeRelativeNTACommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.ChangeDate);
            if (stock != null)
                (stock as StapledSecurity).SetRelativeNTAs(command.ChangeDate, command.Percentages);
        }

        public void Execute(AddNonTradingDayCommand command)
        {
            _StockExchange.TradingCalander.AddNonTradingDay(command.Date);
        }

        public void Execute(AddCapitalReturnCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.RecordDate);
            if (stock != null)
                stock.AddCapitalReturn(command.RecordDate, command.Description, command.PaymentDate, command.Amount);
        }

        public void Execute(AddDividendCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.RecordDate);
            if (stock != null)
                stock.AddDividend(command.RecordDate, command.Description, command.PaymentDate, command.DividendAmount, command.CompanyTaxRate, command.PercentFranked, command.DRPPrice);
        }

    }

}