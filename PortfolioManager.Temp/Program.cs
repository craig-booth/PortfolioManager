using System;
using System.Collections.Generic;
using System.Linq;

using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Stocks.Commands;
using PortfolioManager.Domain.Stocks.Events;

namespace PortfolioManager.Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            var eventStore = new InMemoryEventStore();

            Setup(eventStore);

            Replay(eventStore);

            Console.ReadKey();
        }


        private static void Setup(IEventStore eventStore)
        {
            var stockExchange = new StockExchange(eventStore);

            var commandHandler = new StockExchangeCommandHandler(stockExchange);

            commandHandler.Execute(new ListStockCommand("ARG", "Argo", new DateTime(2000, 01, 01), StockType.Ordinary, AssetCategory.AustralianStocks, RoundingRule.Round, DRPMethod.Round));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 01), 1.00m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 02), 1.01m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 03), 1.02m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 04), 1.03m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 05), 1.01m));
            commandHandler.Execute(new AddClosingPriceCommand("ARG", new DateTime(2000, 01, 06), 1.06m));
        }

        private static void Replay(IEventStore eventStore)
        {
            var stockExchange = new StockExchange(eventStore);

            var events = eventStore.RetrieveEvents(Guid.Empty);
            foreach (var @event in events)
            {
                stockExchange.ApplyEvent(dynamic @event);
            }
        }
    }

    public class StockExchangeCommandHandler :
        ICommandHandler<ListStockCommand>,
        ICommandHandler<DelistStockCommand>,
        ICommandHandler<AddClosingPriceCommand>,
        ICommandHandler<AddNonTradingDayCommand>,
        ICommandHandler<UpdateCurrentPriceCommand>
    {
        private StockExchange _StockExchange;

        public StockExchangeCommandHandler(StockExchange stockExchange)
        {
            _StockExchange = stockExchange;
        }

        public void Execute(ListStockCommand command)
        {
            throw new NotImplementedException();
        }

        public void Execute(DelistStockCommand command)
        {
            throw new NotImplementedException();
        }

        public void Execute(AddClosingPriceCommand command)
        {
            throw new NotImplementedException();
        }

        public void Execute(AddNonTradingDayCommand command)
        {
            throw new NotImplementedException();
        }

        public void Execute(UpdateCurrentPriceCommand command)
        {
            throw new NotImplementedException();
        }
    }

}
