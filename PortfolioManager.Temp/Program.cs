using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Stocks.Commands;

using PortfolioManager.Data.SQLite.Stocks;

namespace PortfolioManager.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventStore = new InMemoryEventStore();
            var stockExchange = new StockExchange(eventStore);

            Load(stockExchange, eventStore);

            var stocks = stockExchange.Stocks.All(DateTime.Today).OrderBy(x => x.Properties[DateTime.Today].ASXCode);
            WritePrices(stocks);


            Console.ReadKey();
        }

        private static void WritePrices(IEnumerable<Stock> stocks)
        {
            foreach (var stock in stocks)
            {
                var properties = stock.Properties[DateTime.Today];
                Console.WriteLine("{0} ({1}): {2}", properties.ASXCode, properties.Name, stock.GetPrice(DateTime.Today));
                
                if (stock.Type == StockType.StapledSecurity)
                {
                    foreach (var childStock in stock.ChildSecurities)
                    {
                        properties = childStock.Properties[DateTime.Today];
                        Console.WriteLine("    {0} ({1})", properties.ASXCode, properties.Name);
                    }
                }

            }
        }


        private static void Load(StockExchange stockExchange, IEventStore eventStore)
        {
            var stockDataBase = new SQLiteStockDatabase(@"C:\PortfolioManager\Stocks.db");

            var commands = new List<ICommand>();

            using (var unitOfWork = stockDataBase.CreateReadOnlyUnitOfWork())
            {
                var stocks = unitOfWork.StockQuery.GetAll().Where(x => x.Type != StockType.StapledSecurity).Select(x => x.Id).Distinct();
                foreach (var stock in stocks)
                    commands.AddRange(CommandsForStock(stock, unitOfWork.StockQuery));

                stocks = unitOfWork.StockQuery.GetAll().Where(x => x.Type == StockType.StapledSecurity).OrderBy(x => x.FromDate).Select(x => x.Id).Distinct().ToList();
                foreach (var stock in stocks)
                    commands.AddRange(CommandsForStock(stock, unitOfWork.StockQuery));
            }

            var commandHandler = new StockExchangeCommandHandler(stockExchange);
            foreach (var command in commands)
            {
                dynamic c = command;
                commandHandler.Execute(c);
            }

        }

        private static IEnumerable<ICommand> CommandsForStock(Guid id, Data.Stocks.IStockQuery stockQuery)
        {
            var commands = new List<ICommand>();

            var stockVersions = stockQuery.GetAll().Where(x => x.Id == id).OrderBy(x => x.FromDate);

            var firstVersion = stockVersions.First();
            if (firstVersion.Type == StockType.StapledSecurity)
            {
                var childSecurities = stockQuery.GetChildStocks(id, firstVersion.FromDate).Select(x => x.ASXCode);
                commands.Add(new ListStockCommand(firstVersion.ASXCode, firstVersion.Name, firstVersion.FromDate, firstVersion.Type, firstVersion.Category, childSecurities));
            }
            else
                commands.Add(new ListStockCommand(firstVersion.ASXCode, firstVersion.Name, firstVersion.FromDate, firstVersion.Type, firstVersion.Category, null));

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
    }

    public class StockExchangeCommandHandler :
        ICommandHandler<ListStockCommand>,
        ICommandHandler<DelistStockCommand>,
        ICommandHandler<AddClosingPriceCommand>,
        ICommandHandler<AddNonTradingDayCommand>,
        ICommandHandler<UpdateCurrentPriceCommand>,
        ICommandHandler<ChangeStockCommand>
    {
        private StockExchange _StockExchange;

        public StockExchangeCommandHandler(StockExchange stockExchange)
        {
            _StockExchange = stockExchange;
        }

        public void Execute(ListStockCommand command)
        {
            IEnumerable<Guid> childSecurities = null;
            if (command.Type == StockType.StapledSecurity)
                childSecurities = command.ChildSecurities.Select(x => _StockExchange.Stocks.Get(x, command.ListingDate).Id);

            _StockExchange.Stocks.ListStock(command.ASXCode, command.Name, command.ListingDate, command.Type, command.Category, childSecurities);
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

        public void Execute(AddNonTradingDayCommand command)
        {
            _StockExchange.TradingCalander.AddNonTradingDay(command.Date);
        }

    }

}
