using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;


using AutoMapper;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using PortfolioManager.Common;
using PortfolioManager.Domain.Stocks;
using PortfolioManager.EventStore;
using PortfolioManager.EventStore.Mongodb;
using PortfolioManager.Data.Portfolios;
using PortfolioManager.Data.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Service.Interface;
using PortfolioManager.Web;
using PortfolioManager.Web.Controllers.v1;
using PortfolioManager.Web.Controllers.v2;
using PortfolioManager.Web.Converters;

namespace PortfolioManager.Test.SystemTests
{
    [TestFixture]
    public class CompareToLive
    {
        private Guid _PortfolioId = Guid.NewGuid();
        private ServiceProvider _ServiceProvider; 

        private string _ExpectedResultsPath;
        private string _ActualResultsPath;

        [OneTimeSetUp]
        public async Task Init()
        {
            _ExpectedResultsPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "ExpectedResults");
            _ActualResultsPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "SystemTests", "ActualResults");
            if (Directory.Exists(_ActualResultsPath))
            {
                var files = Directory.EnumerateFiles(_ActualResultsPath);
                foreach (var file in files)
                    File.Delete(file);
            }
            else
            {
                Directory.CreateDirectory(_ActualResultsPath);
            }

            var settings = new PortfolioManagerSettings()
            {
                ApiKey = Guid.Empty,
                PortfolioDatabase = Path.Combine(_ActualResultsPath, "Portfolio.db"),
                EventStore = "mongodb://ec2-52-62-34-156.ap-southeast-2.compute.amazonaws.com:27017",
                Port = 0
            };
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddPortfolioManagerService(settings);

            services.AddSingleton<Web.Controllers.v2.TransactionController>();
            services.AddSingleton<Web.Controllers.v2.HoldingController>();
            services.AddSingleton<Web.Controllers.v2.CorporateActionController>();

            _ServiceProvider = services.BuildServiceProvider();

            var stockExchange = _ServiceProvider.GetRequiredService<StockExchange>();
            stockExchange.LoadFromEventStream();

            await LoadTransactions();
        }

        public async Task LoadTransactions()
        {
            var service = _ServiceProvider.GetRequiredService<ITransactionService>();
            var transactionConverter = _ServiceProvider.GetRequiredService<TransactionJsonConverter>();

            await service.ImportTransactions(Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "Transactions.xml"));

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" });
            settings.Converters.Add(transactionConverter);
            settings.Converters.Add(new CorporateActionJsonConverter());
            var serializer = JsonSerializer.Create(settings);

            var jsonFile = File.OpenText(Path.Combine(TestContext.CurrentContext.TestDirectory, "SystemTests", "Transactions.json"));
            var jsonReader = new JsonTextReader(jsonFile);
            var transactions = serializer.Deserialize<List<RestApi.Transactions.Transaction>>(jsonReader);
            jsonFile.Close();

            var controller = _ServiceProvider.GetRequiredService<Web.Controllers.v2.TransactionController>(); 
            SetControllerContext(controller);

            foreach (var transaction in transactions)
                controller.AddTransaction(transaction);
        }

        private void SaveActualResult(object actual, string fileName)
        {
            var actualFile = Path.Combine(_ActualResultsPath, fileName);

            using (var streamWriter = new StreamWriter(actualFile))
            {
                var serializer = new XmlSerializer(actual.GetType());   

                serializer.Serialize(streamWriter, actual);
            }
        }

        private void SaveActualResult(object actual, string fileName, Type[] extraTypes)
        {
            var actualFile = Path.Combine(_ActualResultsPath, fileName);

            using (var streamWriter = new StreamWriter(actualFile))
            {
                var serializer = new XmlSerializer(actual.GetType(), extraTypes);

                serializer.Serialize(streamWriter, actual);
            }
        }

        private void SetControllerContext(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            routeData.Values["portfolioId"] = _PortfolioId.ToString();
            var actionDescription = new ActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescription);
            var context = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), controller);

            controller.OnActionExecuting(context);
        } 

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareTransactions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("Transactions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<Web.Controllers.v2.TransactionController>();
            SetControllerContext(controller);

            var response = controller.Get(null, fromDate, toDate);
            SaveActualResult(response.Value, fileName, new Type[]
                        {
                            typeof(RestApi.Transactions.Aquisition),
                            typeof(RestApi.Transactions.CashTransaction),
                            typeof(RestApi.Transactions.CostBaseAdjustment),
                            typeof(RestApi.Transactions.Disposal),
                            typeof(RestApi.Transactions.IncomeReceived),
                            typeof(RestApi.Transactions.OpeningBalance),
                            typeof(RestApi.Transactions.ReturnOfCapital),
                            typeof(RestApi.Transactions.UnitCountAdjustment)
                        });

            UpdateExpectedTransactions(expectedFile);


            Assert.That(response.Value, Is.EquivalentTo(typeof(List<RestApi.Transactions.Transaction>), expectedFile));
        }

        private void UpdateExpectedTransactions(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(GetTransactionsResponce), expectedFile);
            var expected = constraint.Expected as GetTransactionsResponce;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ConvertExpectedResults());
            });
            var mapper = config.CreateMapper();

            var newTransactions = mapper.Map<List<RestApi.Transactions.Transaction>>(expected.Transactions);
            SaveActualResult(newTransactions, expectedFile, new Type[]
                        {
                            typeof(RestApi.Transactions.Aquisition),
                            typeof(RestApi.Transactions.CashTransaction),
                            typeof(RestApi.Transactions.CostBaseAdjustment),
                            typeof(RestApi.Transactions.Disposal),
                            typeof(RestApi.Transactions.IncomeReceived),
                            typeof(RestApi.Transactions.OpeningBalance),
                            typeof(RestApi.Transactions.ReturnOfCapital),
                            typeof(RestApi.Transactions.UnitCountAdjustment)
                        });

        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task CompareCapitalGain(DateTime date)
        {
            var fileName = String.Format("CapitalGain {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetDetailedCapitalGains(null, date);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(DetailedUnrealisedGainsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareCGTLiability(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CGTLiability {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetCGTLiability(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(CGTLiabilityResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareCashTransactions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CashTransactions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetCashAccountTransactions(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(CashAccountTransactionsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public void CompareHoldings(DateTime date)
        {
            var fileName = String.Format("Holdings {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = _ServiceProvider.GetRequiredService<Web.Controllers.v2.HoldingController>();
            SetControllerContext(controller);

            var response = controller.Get(null, date);
            SaveActualResult(response.Value, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(HoldingsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task CompareTradeableHoldings(DateTime date)
        {
            var fileName = String.Format("TradeableHoldings {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetHoldings(date, true);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(HoldingsResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task CompareIncome(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("Income {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetIncome(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(IncomeResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task ComparePortfolioPerformance(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioPerformance {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetPerformance(fromDate, toDate);
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(PortfolioPerformanceResponce), expectedFile));
        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDates")]
        public async Task ComparePortfolioSummary(DateTime date)
        {
            var fileName = String.Format("PortfolioSummary {0:yyy-MM-dd}.xml", date);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetSummary(date);
            SaveActualResult(response, fileName);

            UpdateExpectedPortfolioSummary(expectedFile);

            Assert.That(response, Is.EquivalentTo(typeof(PortfolioSummaryResponce), expectedFile));
        }

        private void UpdateExpectedPortfolioSummary(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(PortfolioSummaryResponce), expectedFile);
            var expected = constraint.Expected as PortfolioSummaryResponce;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ConvertExpectedResults());
            });
            var mapper = config.CreateMapper();

            var newResponse = mapper.Map<RestApi.Portfolios.PortfolioSummaryResponse>(expected);
            var newHoldings = mapper.Map<List<RestApi.Portfolios.Holding>>(expected.Holdings);
            newResponse.Holdings.AddRange(newHoldings);
            SaveActualResult(newResponse, expectedFile);

        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public async Task ComparePortfolioValue(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("PortfolioValue {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetPortfolioValue(null, fromDate, toDate, ValueFrequency.Daily);
            SaveActualResult(response, fileName);

            UpdateExpectedPortfolioValue(expectedFile);

            Assert.That(response, Is.EquivalentTo(typeof(PortfolioValueResponce), expectedFile));
        }

        private void UpdateExpectedPortfolioValue(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(PortfolioValueResponce), expectedFile);
            var expected = constraint.Expected as PortfolioValueResponce;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ConvertExpectedResults());
            });
            var mapper = config.CreateMapper();

            var newResponse = mapper.Map<RestApi.Portfolios.PortfolioValueResponse>(expected);
            SaveActualResult(newResponse, expectedFile);

        }

        [Test, TestCaseSource(typeof(CompareToLiveTestData), "TestDateRanges")]
        public void CompareCorporateActions(DateTime fromDate, DateTime toDate)
        {
            var fileName = String.Format("CorporateActions {0:yyy-MM-dd}.xml", toDate);
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            CorporateActionsResponce response = new CorporateActionsResponce();


            var controller = _ServiceProvider.GetRequiredService<CorporateActionController>();

            var stockRepository = _ServiceProvider.GetRequiredService<Domain.Stocks.IStockRepository>();
            var stocks = stockRepository.All().Where(x => x.IsEffectiveDuring(new DateRange(fromDate, toDate))).OrderBy(x => x, new StockComparer());
            foreach (var stock in stocks)
            {
                var stockItem = new StockItem(stock.Id, stock.Properties.ClosestTo(toDate).ASXCode, stock.Properties.ClosestTo(toDate).Name);

                var result = controller.GetCorporateActions(stock.Id, fromDate, toDate);

                if (result.Value != null)
                {
                    var corporateActions = result.Value.Select(x => new CorporateActionItem()
                    {
                        Id = x.Id,
                        ActionDate = x.ActionDate,
                        Stock = stockItem,
                        Description = x.Description
                    });

                    response.CorporateActions.AddRange(corporateActions);
                }

            }

            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(CorporateActionsResponce), expectedFile));
        }

        [Test]
        public async Task CompareUnappliedCorporateActions()
        {
            var fileName = "UnappliedCorporateActions.xml";
            var expectedFile = Path.Combine(_ExpectedResultsPath, fileName);

            var controller = new PortfolioController(_ServiceProvider);
            var response = await controller.GetUnappliedCorporateActions();
            SaveActualResult(response, fileName);

            Assert.That(response, Is.EquivalentTo(typeof(UnappliedCorporateActionsResponce), expectedFile));
        }

    }

    public class CompareToLiveTestData
    {
        public static IEnumerable TestDates
        {
            get
            {
                var date = new DateTime(2016, 01, 01);
                while (date.Year <= 2017)
                {
                    var testData = new TestCaseData(date).SetName("{m} (" + date.ToString("yyyy-MM-dd") + ")");       
                    date = date.AddMonths(1);

                    yield return testData;
                }
            }
        }

        public static IEnumerable TestDateRanges
        {
            get
            {
                var date = new DateTime(2016, 01, 01);
                while (date.Year <= 2017)
                {
                    var fromDate = date.AddYears(-1).AddDays(1);
                    var testData = new TestCaseData(fromDate, date).SetName("{m} (" + fromDate.ToString("yyyy-MM-dd") + " to " + date.ToString("yyyy-MM-dd") + ")");
                    date = date.AddMonths(1);

                    yield return testData;
                }
            }
        }
    }

    class StockComparer : IComparer<Domain.Stocks.Stock>
    {
        static List<string> Codes = new List<string>() { "ARG", "AMP", "NAB", "TLS", "AGI", "TWE", "CSL" , "BHP", "COH", "WAM", "GVF", "S32", "VGS", "VAF", "VAP", "VCF"};

        public int Compare(Domain.Stocks.Stock stock1, Domain.Stocks.Stock stock2)
        {
            var code1 = stock1.Properties.ClosestTo(DateTime.Today).ASXCode;
            var code2 = stock2.Properties.ClosestTo(DateTime.Today).ASXCode;

            var i1 = Codes.IndexOf(code1);
            var i2 = Codes.IndexOf(code2);

            if ((i1 == -1) && (i2 == -1))
                return code1.CompareTo(code2);
            else if (i1 == -1)
                return 1;
            else if (i2 == -1)
                return -1;
            else
                return i1.CompareTo(i2);
        }
    }

    public class ConvertExpectedResults : Profile
    {
        public ConvertExpectedResults()
        {
            CreateMap<TransactionItem, RestApi.Transactions.Transaction>()
                .ForMember(x => x.Stock, x => x.MapFrom(y => y.Stock.Id))
                .Include<AquisitionTransactionItem, RestApi.Transactions.Aquisition>()
                .Include<CashTransactionItem, RestApi.Transactions.CashTransaction>()
                .Include<CostBaseAdjustmentTransactionItem, RestApi.Transactions.CostBaseAdjustment>()
                .Include<DisposalTransactionItem, RestApi.Transactions.Disposal>()
                .Include<IncomeTransactionItem, RestApi.Transactions.IncomeReceived>()
                .Include<OpeningBalanceTransactionItem, RestApi.Transactions.OpeningBalance>()
                .Include<ReturnOfCapitalTransactionItem, RestApi.Transactions.ReturnOfCapital>()
                .Include<UnitCountAdjustmentTransactionItem, RestApi.Transactions.UnitCountAdjustment>();
            CreateMap<AquisitionTransactionItem, RestApi.Transactions.Aquisition>();
            CreateMap<CashTransactionItem, RestApi.Transactions.CashTransaction>();
            CreateMap<CostBaseAdjustmentTransactionItem, RestApi.Transactions.CostBaseAdjustment>();
            CreateMap<DisposalTransactionItem, RestApi.Transactions.Disposal>();
            CreateMap<IncomeTransactionItem, RestApi.Transactions.IncomeReceived>();
            CreateMap<OpeningBalanceTransactionItem, RestApi.Transactions.OpeningBalance>();
            CreateMap<ReturnOfCapitalTransactionItem, RestApi.Transactions.ReturnOfCapital>();
            CreateMap<UnitCountAdjustmentTransactionItem, RestApi.Transactions.UnitCountAdjustment>();


            CreateMap<PortfolioSummaryResponce, RestApi.Portfolios.PortfolioSummaryResponse>();
            CreateMap<HoldingItem, RestApi.Portfolios.Holding>()
                .AfterMap((src, dest) => dest.Stock.Category = src.Category);
            CreateMap<StockItem, RestApi.Portfolios.Stock>();

            CreateMap<PortfolioValueResponce, RestApi.Portfolios.PortfolioValueResponse>();

        }
    }
}
