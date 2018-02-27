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

            var stockDataBase = new SQLiteStockDatabase(@"C:\PortfolioManager\Stocks.db");

            var commands = new List<ICommand>();

            using (var unitOfWork = stockDataBase.CreateReadOnlyUnitOfWork())
            {
                
                Data.Stocks.Stock previousStock = null;
           
                var stocks = unitOfWork.StockQuery.GetAll().OrderBy(x => x.Id).ThenBy(x => x.FromDate);
                foreach (var stock in stocks)
                {
                    
                    if ((previousStock != null) && (stock.Id == previousStock.Id))
                    {
                        commands.Add(new ChangeStockCommand(previousStock.ASXCode, stock.FromDate, stock.ASXCode, stock.Name));
                    }
                    else
                    {
                        if ((previousStock != null) && (previousStock.ToDate != DateUtils.NoEndDate))
                        {
                            commands.Add(new DelistStockCommand(previousStock.ASXCode, previousStock.ToDate));
                        }

                        commands.Add(new ListStockCommand(stock.ASXCode, stock.Name, stock.FromDate, stock.Type, stock.Category));
                    }

                    previousStock = stock;
                }
                
            }

            var commandHandler = new StockExchangeCommandHandler(stockExchange);
            foreach (var command in commands)
            {
                dynamic c = command;
                commandHandler.Execute(c);
            }

            var stockList = stockExchange.Stocks.All(DateTime.Today).Select(x => x.GetProperties(DateTime.Today)).OrderBy(x => x.ASXCode);
            foreach (var properties in stockList)
            {
                Console.WriteLine("{0} - {1}", properties.ASXCode, properties.Name);
            }



            Console.ReadKey();
        }


        private static void Setup(IEventStore eventStore)
        {
            var stockExchange = new StockExchange(eventStore);

            var commandHandler = new StockExchangeCommandHandler(stockExchange);

            commandHandler.Execute(new ListStockCommand("ARG", "Argo", new DateTime(2000, 01, 01), StockType.Ordinary, AssetCategory.AustralianStocks));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 01), 1.00m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 02), 1.01m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 03), 1.02m));
            commandHandler.Execute(new ChangeStockCommand("ARG", new DateTime(2000, 01, 04), "ARG2", "New Argo"));
            commandHandler.Execute(new AddClosingPriceCommand("ARG2", new DateTime(2000, 01, 04), 1.03m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG2", new DateTime(2000, 01, 05), 1.01m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG2", new DateTime(2000, 01, 06), 1.06m));
        }

        private static void Replay(IEventStore eventStore)
        {
            var stockExchange = new StockExchange(eventStore);
            
            stockExchange.Load();
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
            _StockExchange.Stocks.ListStock(command.ASXCode, command.Name, command.ListingDate, command.Type, command.Category);
        }

        public void Execute(ChangeStockCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.ChangeDate);
            if (stock != null)
                stock.ChangeName(command.ChangeDate, command.NewASXCode, command.Name);
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
