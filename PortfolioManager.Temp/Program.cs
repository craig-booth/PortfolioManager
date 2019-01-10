using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using PortfolioManager.Common.Scheduler;
using PortfolioManager.Common;
using PortfolioManager.Domain;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.Domain.Stocks.Events;
using PortfolioManager.Domain.Portfolios;
using PortfolioManager.Domain.CorporateActions.Events;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Memory;
using PortfolioManager.EventStore.Sqlite;
using PortfolioManager.EventStore.Mongodb;
using PortfolioManager.Service.Services;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.RestApi.Client;
using PortfolioManager.RestApi.Stocks;
using PortfolioManager.RestApi.TradingCalander;
using PortfolioManager.ImportData.DataServices;
using PortfolioManager.Service.Transactions;

namespace PortfolioManager.Temp
{
    class Program
    {
        static void Main(string[] args)
        {


            //   ConvertToEventSourcedModel();
            //   Test();
            //    TestScheduler();

            // this line is needed to ensure that the assembly for events is loaded
            //var xx = new StockListedEvent(Guid.Empty, 0, "", "", DateTime.Today, Common.AssetCategory.AustralianStocks, false);

            //MigrateDatabase();

            //var mongoEventStore = new MongodbEventStore("mongodb://192.168.99.100:27017");
            //      var mongoEventStore = new MongodbEventStore("mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017");
            //      CreateMongoDBEventStore(mongoEventStore, @"C:\PortfolioManager\Database\v3_0\Events.db");

            //      Test(mongoEventStore);

            //  var mongoEventStore = new MongodbEventStore("mongodb://192.168.99.100:27017");
            //  PortfolioTest(mongoEventStore);

            ConvertTransactionsFile();
        }

        public static void ConvertTransactionsFile()
        {
            var mongoEventStore = new MongodbEventStore("mongodb://192.168.99.100:27017");
            var stockExchange = new StockExchange(mongoEventStore);
            stockExchange.LoadFromEventStream();

            var importer = new TransactionImporter();

            var fileName = @"C:\Users\Craig\Source\Repos\PortfolioManager\PortfolioManager.Test\SystemTests\Transactions.xml";
            var oldTransactions = importer.ImportTransactions(fileName);
         
            var newTransactions = new List<RestApi.Transactions.Transaction>();
            foreach (var oldTransaction in oldTransactions)
            {
                RestApi.Transactions.Transaction newTransaction = null;
                if (oldTransaction.Type == TransactionType.Aquisition)
                {
                    var aquisition = oldTransaction as Data.Portfolios.Aquisition;
                    newTransaction = new RestApi.Transactions.Aquisition()
                    {
                        Units = aquisition.Units,
                        AveragePrice = aquisition.AveragePrice,
                        TransactionCosts = aquisition.TransactionCosts,
                        CreateCashTransaction = aquisition.CreateCashTransaction
                    };
                }
                else if (oldTransaction.Type == TransactionType.CashTransaction)
                {
                    var cashTransaction = oldTransaction as Data.Portfolios.CashTransaction;

                    string type = "";
                    if (cashTransaction.CashTransactionType == BankAccountTransactionType.Deposit)
                        type = "deposit";
                    else if (cashTransaction.CashTransactionType == BankAccountTransactionType.Fee)
                        type = "fee";
                    else if (cashTransaction.CashTransactionType == BankAccountTransactionType.Interest)
                        type = "interest";
                    else if (cashTransaction.CashTransactionType == BankAccountTransactionType.Transfer)
                        type = "transfer";
                    else if (cashTransaction.CashTransactionType == BankAccountTransactionType.Withdrawl)
                        type = "withdrawl";

                    newTransaction = new RestApi.Transactions.CashTransaction()
                    {
                        CashTransactionType = type,
                        Amount = cashTransaction.Amount
                    };
                }
                else if (oldTransaction.Type == TransactionType.CostBaseAdjustment)
                {
                    var adjustment = oldTransaction as Data.Portfolios.CostBaseAdjustment;
                    newTransaction = new RestApi.Transactions.CostBaseAdjustment()
                    {
                        Percentage = adjustment.Percentage
                    };
                }
                else if (oldTransaction.Type == TransactionType.Disposal)
                {
                    var disposal = oldTransaction as Data.Portfolios.Disposal;

                    string method = "";
                    if (disposal.CGTMethod == CGTCalculationMethod.FirstInFirstOut)
                        method = "fifo";
                    else if (disposal.CGTMethod == CGTCalculationMethod.LastInFirstOut)
                        method = "lifo";
                    else if (disposal.CGTMethod == CGTCalculationMethod.MaximizeGain)
                        method = "maximise";
                    else if (disposal.CGTMethod == CGTCalculationMethod.MinimizeGain)
                        method = "minimize";

                    newTransaction = new RestApi.Transactions.Disposal()
                    {
                        Units = disposal.Units,
                        AveragePrice = disposal.Units,
                        TransactionCosts = disposal.TransactionCosts,
                        CGTMethod = method,
                        CreateCashTransaction = disposal.CreateCashTransaction
                    };
                }
                else if (oldTransaction.Type == TransactionType.Income)
                {
                    var income = oldTransaction as Data.Portfolios.IncomeReceived;
                    newTransaction = new RestApi.Transactions.IncomeReceived()
                    {
                        FrankedAmount = income.FrankedAmount,
                        UnfrankedAmount = income.UnfrankedAmount,
                        FrankingCredits = income.FrankingCredits,
                        Interest = income.Interest,
                        TaxDeferred = income.TaxDeferred,
                        CreateCashTransaction = income.CreateCashTransaction,
                        DRPCashBalance = income.DRPCashBalance
                    };
                }
                else if (oldTransaction.Type == TransactionType.OpeningBalance)
                {
                    var openingBalance = oldTransaction as Data.Portfolios.OpeningBalance;
                    newTransaction = new RestApi.Transactions.OpeningBalance()
                    {
                        Units = openingBalance.Units,
                        CostBase = openingBalance.CostBase,
                        AquisitionDate = openingBalance.AquisitionDate
                    };
                }
                else if (oldTransaction.Type == TransactionType.ReturnOfCapital)
                {
                    var capitalReturn = oldTransaction as Data.Portfolios.ReturnOfCapital;
                    newTransaction = new RestApi.Transactions.ReturnOfCapital()
                    {
                        Amount = capitalReturn.Amount,
                        CreateCashTransaction = capitalReturn.CreateCashTransaction
                    };
                }
                else if (oldTransaction.Type == TransactionType.UnitCountAdjustment)
                {
                    var adjustment = oldTransaction as Data.Portfolios.UnitCountAdjustment;
                    newTransaction = new RestApi.Transactions.UnitCountAdjustment()
                    {
                        OriginalUnits = adjustment.OriginalUnits,
                        NewUnits = adjustment.NewUnits
                    };
                }

                Guid stockId;
                if (oldTransaction.Type != TransactionType.CashTransaction)
                {
                    var stock = stockExchange.Stocks.Get(oldTransaction.ASXCode, oldTransaction.TransactionDate);
                    stockId = stock.Id;
                }
                else
                    stockId = Guid.Empty;

                newTransaction.Id = oldTransaction.Id;
                newTransaction.Stock = stockId;
                newTransaction.RecordDate = oldTransaction.RecordDate;
                newTransaction.TransactionDate = oldTransaction.TransactionDate;
                newTransaction.Comment = oldTransaction.Comment;

                newTransactions.Add(newTransaction);
            }

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });

            var json = JsonConvert.SerializeObject(newTransactions, settings);

            var newFile = @"C:\Users\Craig\Source\Repos\PortfolioManager\PortfolioManager.Test\SystemTests\Transactions2.json";
            var jsonFile = File.CreateText(newFile);
            jsonFile.Write(json);
            jsonFile.Close();
        }

        public static void PortfolioTest(IEventStore eventStore)
        {
            var stockExchange = new StockExchange(eventStore);

            stockExchange.LoadFromEventStream();

            var stock = stockExchange.Stocks.Get("ARG", DateTime.Today);

            var portfolio = new Portfolio(Guid.NewGuid(), "Test Portfolio", null);

    /*        portfolio.CashAccount.Deposit(new DateTime(2018, 01, 01), 40000.00m, "Initial deposit");

            portfolio.PurchaseStock(stock, new DateTime(2018, 01, 01), 1000, 7.00m, 19.95m);
            portfolio.CashAccount.Withdraw(new DateTime(2018, 01, 01), 1000 * 7.00m, "Purchase 1,000 ARG @ $7.00");
            portfolio.CashAccount.FeeDeducted(new DateTime(2018, 01, 01), 19.95m, "Brokerage for ARG purchase");

            portfolio.PurchaseStock(stock, new DateTime(2018, 03, 01), 500, 7.50m, 19.95m);
            portfolio.CashAccount.Withdraw(new DateTime(2018, 03, 01), 500 * 7.50m, "Purchase 500 ARG @ $7.50");
            portfolio.CashAccount.FeeDeducted(new DateTime(2018, 03, 01), 19.95m, "Brokerage for ARG purchase");

            portfolio.PurchaseStock(stock, new DateTime(2018, 06, 01), 200, 7.20m, 19.95m);
            portfolio.CashAccount.Withdraw(new DateTime(2018, 06, 01), 200 * 7.20m, "Purchase 200 ARG @ $7.20");
            portfolio.CashAccount.FeeDeducted(new DateTime(2018, 06, 01), 19.95m, "Brokerage for ARG purchase");

            var holding = portfolio.GetHolding(stock);

            foreach (var property in holding.Properties.Values.Reverse())
            {
                Console.WriteLine("{0} : {1}, {2}", property.EffectivePeriod.FromDate, property.Properties.Units, property.Properties.CostBase);
                var balance = portfolio.CashAccount[property.EffectivePeriod.FromDate];
                Console.WriteLine("   {0}", balance);
            }

            Console.ReadKey(); */
        }

        public static void MigrateDatabase()
        {
            var stockDatabase = new SQLiteStockDatabase(@"C:\PortfolioManager\Stocks.db");
            // RestClient restClient = null;
            // RestClient restClient = new RestClient("http://localhost", Guid.Empty);
        //    RestClient restClient = new RestClient("https://docker.local:8443", new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D"));
            RestClient restClient = new RestClient("https://portfolio.boothfamily.id.au", new Guid("B34A4C8B-6B17-4E25-A3CC-2E512D5F1B3D"));
            var migrator = new MigrateDatabase(stockDatabase, restClient);

            var loadCalanderTask = migrator.LoadTradingCalander();
            Task.WaitAll(loadCalanderTask);

            var stocks = migrator.ListStocks();
            foreach (var stock in stocks)
            {
                var loadStockTask = migrator.LoadStock(stock);
                Task.WaitAll(loadStockTask);
            }

        }

        public static void CreateMongoDBEventStore(MongodbEventStore mongoEventStore, string sqliteFile)
        {
            var sqliteEventStore = new SqliteEventStore(sqliteFile);

            mongoEventStore.CopyEventStream(sqliteEventStore, "TradingCalander");
            mongoEventStore.CopyEventStream(sqliteEventStore, "StockRepository");
        }

        private static void TestScheduler()
        {
            var scheduler = new Scheduler();

            var t = scheduler.Run();

            scheduler.Add("", Job1, Schedule.Daily().Every(1, TimeUnit.Minutes), DateTime.Now);
            scheduler.Add("", Job2, Schedule.Daily().Every(2, TimeUnit.Minutes), DateTime.Now);
            scheduler.Add("", Job5, Schedule.Daily().Every(5, TimeUnit.Minutes), DateTime.Now);
            
            t.Wait();
        }

        private static void Job1()
        {
            Console.WriteLine("Job 1: " + DateTime.Now.ToShortTimeString());
        }

        private static async void Job2()
        {
            Console.WriteLine("Job 2 start: " + DateTime.Now.ToShortTimeString());
            await Task.Delay(90000);
            Console.WriteLine("Job 2 end: " + DateTime.Now.ToShortTimeString());
        }

        private static void Job5()
        {
            Console.WriteLine("Job 5: " + DateTime.Now.ToShortTimeString());
        }

    /*    private static void ConvertToEventSourcedModel()
        {
            var eventStore = new MemoryEventStore();
            var stockExchange = new StockExchange(eventStore);

            Load(stockExchange);
            LoadCorporateActions(stockExchange);

            // Copy event to sqlite database 
            var sqliteEventStore = new SqliteEventStore(@"C:\PortfolioManager\Events.db");

            eventStore.CopyEventStream(sqliteEventStore, "TradingCalander");
            eventStore.CopyEventStream(sqliteEventStore, "StockRepository");
        } */

        private static void Test(IEventStore eventStore)
        {
            var stockExchange = new StockExchange(eventStore);

            stockExchange.LoadFromEventStream();

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
/*
        private static void LoadCorporateActions(StockExchange stockExchange)
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

        private static void Load(StockExchange stockExchange)
        {
            var stockDataBase = new SQLiteStockDatabase(@"C:\PortfolioManager\Stocks.db");

            var commands = new List<object>();

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

        private static IEnumerable<object> LoadStocks(Data.Stocks.IStockQuery stockQuery)
        {
            var commands = new List<object>();

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

        private static IEnumerable<object> CommandsForStock(Guid id, Data.Stocks.IStockQuery stockQuery)
        {
            var commands = new List<object>();

            var stockVersions = stockQuery.GetAll().Where(x => x.Id == id).OrderBy(x => x.FromDate);
            /*
            var firstVersion = stockVersions.First();
            if (firstVersion.Type == StockType.StapledSecurity)
            {
                var childSecurities = stockQuery.GetChildStocks(id, firstVersion.FromDate).Select(x => new CreateStapledSecurityCommand.StapledSecurityChild(x.ASXCode, x.Name, (x.Type == StockType.Trust)));
                commands.Add(new CreateStapledSecurityCommand(id, firstVersion.FromDate, firstVersion.ASXCode, firstVersion.Name, firstVersion.Category, childSecurities));
            }
            else
            {
                var trust = (firstVersion.Type == StockType.Trust);
                commands.Add(new CreateStockCommand(id, firstVersion.FromDate, firstVersion.ASXCode, firstVersion.Name, trust, firstVersion.Category));
            }

            foreach (var otherVersion in stockVersions.Skip(1))
            {
                commands.Add(new ChangeStockCommand(id, otherVersion.FromDate, otherVersion.ASXCode, otherVersion.Name, otherVersion.Category));
            }

            var lastVersion = stockVersions.Last();
            if (lastVersion.ToDate != DateUtils.NoEndDate)
            {
                commands.Add(new DelistStockCommand(id, lastVersion.ToDate));
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
        } */
    }

    public class StockExchangeCommandHandler 
    {
        private StockExchange _StockExchange;

        public StockExchangeCommandHandler(StockExchange stockExchange)
        {
            _StockExchange = stockExchange;
        }

        public void Execute(CreateStockCommand command)
        {
            _StockExchange.Stocks.ListStock(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Trust, command.Category);
        }

  /*      public void Execute(CreateStapledSecurityCommand command)
        {
            var childSecurities = command.ChildSecurities.Select(x => new StapledSecurityChild(x.ASXCode, x.Name, x.Trust));
            _StockExchange.Stocks.ListStapledSecurity(command.Id, command.AsxCode, command.Name, command.ListingDate, command.Category, childSecurities);
        }
        */
    /*    public void Execute(ChangeStockCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.Id);
            if (stock != null)
                stock.ChangeProperties(command.ChangeDate, command.AsxCode, command.Name, command.Category);
        }

        public void Execute(ChangeDividendReinvestmentPlanCommand command)
        {
            var stock = _StockExchange.Stocks.Get(command.ASXCode, command.ChangeDate);
            if (stock != null)
                stock.ChangeDRPRules(command.ChangeDate, command.DRPActive, command.RoundingRule, command.Method);
        }

        public void Execute(DelistStockCommand command)
        {
            _StockExchange.Stocks.DelistStock(command.Id, command.DelistingDate);
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
        } */

      /*  public void Execute(AddNonTradingDayCommand command)
        {
            _StockExchange.TradingCalander.AddNonTradingDay(command.Date);
        }
        */
   /*     public void Execute(AddCapitalReturnCommand command)
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
        } */

    }


 
}
