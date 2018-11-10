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
using PortfolioManager.Service.Interface;
using PortfolioManager.Web;
using PortfolioManager.Web.Controllers.v1;
using PortfolioManager.Web.Controllers.v2;
using PortfolioManager.Web.Converters;

namespace PortfolioManager.Test.SystemTests
{
    public class ConvertToV2
    {
        private IMapper _Mapper;

        public ConvertToV2()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ConvertExpectedResults());
            });
            _Mapper = config.CreateMapper();
        }

        private void SaveResult(object actual, string fileName)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(actual.GetType());

                serializer.Serialize(streamWriter, actual);
            }
        }

        private void SaveResult(object actual, string fileName, Type[] extraTypes)
        {
            using (var streamWriter = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(actual.GetType(), extraTypes);

                serializer.Serialize(streamWriter, actual);
            }
        }

        public void UpdateExpectedTransactions(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(GetTransactionsResponce), expectedFile);
            var expected = constraint.Expected as GetTransactionsResponce;

            var newTransactions = _Mapper.Map<RestApi.Portfolios.TransactionsResponse>(expected);

            SaveResult(newTransactions, expectedFile, new Type[]
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

        public void UpdateExpectedCapitalGain(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(DetailedUnrealisedGainsResponce), expectedFile);
            var expected = constraint.Expected as DetailedUnrealisedGainsResponce;

            var newResponse = _Mapper.Map<RestApi.Portfolios.DetailedUnrealisedGainsResponse>(expected);
            foreach (var item in expected.CGTItems)
            {
                var newItem = _Mapper.Map<RestApi.Portfolios.DetailedUnrealisedGainsItem>(item);
                foreach (var cgtEvent in item.CGTEvents)
                {
                    var newEvent = _Mapper.Map<RestApi.Portfolios.CGTEventItem>(cgtEvent);
                    newItem.CGTEvents.Add(newEvent);
                }
                newResponse.UnrealisedGains.Add(newItem);
            }

            SaveResult(newResponse, expectedFile);
        }

        public void UpdateExpectedCGTLiability(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(CGTLiabilityResponce), expectedFile);
            var expected = constraint.Expected as CGTLiabilityResponce;

            var newResponse = _Mapper.Map<RestApi.Portfolios.CGTLiabilityResponse>(expected);
            var newEvents = _Mapper.Map<List<RestApi.Portfolios.CGTLiabilityResponse.CGTLiabilityEvent>>(expected.Items);
            newResponse.Events.AddRange(newEvents);

            SaveResult(newResponse, expectedFile);
        }

        public void UpdateExpectedCashTransactions(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(CashAccountTransactionsResponce), expectedFile);
            var expected = constraint.Expected as CashAccountTransactionsResponce;

            var newResponse = _Mapper.Map<RestApi.Portfolios.CashAccountTransactionsResponse>(expected);
            var newTransactions = _Mapper.Map<List<RestApi.Portfolios.CashAccountTransactionsResponse.TransactionItem>>(expected.Transactions);
            newResponse.Transactions.AddRange(newTransactions);

            SaveResult(newResponse, expectedFile);
        }

        public void UpdateExpectedPortfolioPerformance(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(PortfolioPerformanceResponce), expectedFile);
            var expected = constraint.Expected as PortfolioPerformanceResponce;

            var newResponse = _Mapper.Map<RestApi.Portfolios.PortfolioPerformanceResponse>(expected);
            var holdingPerformance = _Mapper.Map<List<RestApi.Portfolios.PortfolioPerformanceResponse.HoldingPerformanceItem>>(expected.HoldingPerformance);         
            newResponse.HoldingPerformance.AddRange(holdingPerformance);

            SaveResult(newResponse, expectedFile);
        }


        public void UpdateExpectedPortfolioSummary(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(PortfolioSummaryResponce), expectedFile);
            var expected = constraint.Expected as PortfolioSummaryResponce;

            var newResponse = _Mapper.Map<RestApi.Portfolios.PortfolioSummaryResponse>(expected);
            var newHoldings = _Mapper.Map<List<RestApi.Portfolios.Holding>>(expected.Holdings);
            newResponse.Holdings.AddRange(newHoldings);
            SaveResult(newResponse, expectedFile);
        }

        public void UpdateExpectedPortfolioValue(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(PortfolioValueResponce), expectedFile);
            var expected = constraint.Expected as PortfolioValueResponce;

            var newResponse = _Mapper.Map<RestApi.Portfolios.PortfolioValueResponse>(expected);
            SaveResult(newResponse, expectedFile);
        }

        public void UpdateExpectedIncome(string expectedFile)
        {
            var constraint = new PortfolioResponceContraint(typeof(IncomeResponce), expectedFile);
            var expected = constraint.Expected as IncomeResponce;

            var newResponse = _Mapper.Map<RestApi.Portfolios.IncomeResponse>(expected);
            var newIncome = _Mapper.Map<List<RestApi.Portfolios.IncomeResponse.IncomeItem>>(expected.Income);
            newResponse.Income.AddRange(newIncome);
            SaveResult(newResponse, expectedFile);
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

            CreateMap<GetTransactionsResponce, RestApi.Portfolios.TransactionsResponse>();
            CreateMap<TransactionItem, RestApi.Portfolios.TransactionsResponse.TransactionItem>(); 

            CreateMap<PortfolioSummaryResponce, RestApi.Portfolios.PortfolioSummaryResponse>();
            CreateMap<HoldingItem, RestApi.Portfolios.Holding>()
                .AfterMap((src, dest) => dest.Stock.Category = src.Category);
            CreateMap<StockItem, RestApi.Portfolios.Stock>();

            CreateMap<PortfolioValueResponce, RestApi.Portfolios.PortfolioValueResponse>();

            CreateMap<PortfolioPerformanceResponce, RestApi.Portfolios.PortfolioPerformanceResponse>();
            CreateMap<HoldingPerformance, RestApi.Portfolios.PortfolioPerformanceResponse.HoldingPerformanceItem>();

            CreateMap<CashAccountTransactionsResponce, RestApi.Portfolios.CashAccountTransactionsResponse>();
            CreateMap<CashAccountTransactionItem, RestApi.Portfolios.CashAccountTransactionsResponse.TransactionItem>();

            CreateMap<DetailedUnrealisedGainsResponce, RestApi.Portfolios.DetailedUnrealisedGainsResponse>();
            CreateMap<DetailedUnrealisedGainsItem, RestApi.Portfolios.DetailedUnrealisedGainsItem>();
            CreateMap<CGTEventItem, RestApi.Portfolios.CGTEventItem>();

            CreateMap<CGTLiabilityResponce, RestApi.Portfolios.CGTLiabilityResponse>();
            CreateMap<CGTLiabilityItem, RestApi.Portfolios.CGTLiabilityResponse.CGTLiabilityEvent>();

            CreateMap<IncomeResponce, RestApi.Portfolios.IncomeResponse>();
            CreateMap<IncomeItem, RestApi.Portfolios.IncomeResponse.IncomeItem>();
        }
    }
}
