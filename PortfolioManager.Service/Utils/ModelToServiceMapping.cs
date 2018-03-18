using System;
using System.Collections.Generic;
using System.Text;

using AutoMapper;

using PortfolioManager.Data.Portfolios;
using PortfolioManager.Service.Interface;
using PortfolioManager.Domain.Stocks;

namespace PortfolioManager.Service.Utils
{
    class ModelToServiceMapping : Profile
    {
        public ModelToServiceMapping(StockExchange stockExchange)
        {
            var stockResolver = new StockResolver(stockExchange);

            CreateMap<Transaction, TransactionItem>()
                .ForMember(dest => dest.Stock, opts => opts.ResolveUsing(stockResolver))
                .Include<Aquisition, AquisitionTransactionItem>()
                .Include<CashTransaction, CashTransactionItem>()
                .Include<CostBaseAdjustment, CostBaseAdjustmentTransactionItem>()
                .Include<Disposal, DisposalTransactionItem>()
                .Include<IncomeReceived, IncomeTransactionItem>()
                .Include<OpeningBalance, OpeningBalanceTransactionItem>()
                .Include<ReturnOfCapital, ReturnOfCapitalTransactionItem>()
                .Include<UnitCountAdjustment, UnitCountAdjustmentTransactionItem>();
            CreateMap<Aquisition, AquisitionTransactionItem>();
            CreateMap<CashTransaction, CashTransactionItem>();
            CreateMap<CostBaseAdjustment, CostBaseAdjustmentTransactionItem>();
            CreateMap<Disposal, DisposalTransactionItem>();
            CreateMap<IncomeReceived, IncomeTransactionItem>();
            CreateMap<OpeningBalance, OpeningBalanceTransactionItem>();
            CreateMap<ReturnOfCapital, ReturnOfCapitalTransactionItem>();
            CreateMap<UnitCountAdjustment, UnitCountAdjustmentTransactionItem>();

            CreateMap<TransactionItem, Transaction>()
                .ForMember(dest => dest.ASXCode, opts => opts.MapFrom(src => src.Stock.ASXCode))
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .Include<AquisitionTransactionItem, Aquisition>()
                .Include<CashTransactionItem, CashTransaction>()
                .Include<CostBaseAdjustmentTransactionItem, CostBaseAdjustment>()
                .Include<DisposalTransactionItem, Disposal>()
                .Include<IncomeTransactionItem, IncomeReceived>()
                .Include<OpeningBalanceTransactionItem, OpeningBalance>()
                .Include<ReturnOfCapitalTransactionItem, ReturnOfCapital>()
                .Include<UnitCountAdjustmentTransactionItem, UnitCountAdjustment>();
            CreateMap<AquisitionTransactionItem, Aquisition>()
                .ConstructUsing(src => new Aquisition(src.Id));
            CreateMap<CashTransactionItem, CashTransaction>()
                .ConstructUsing(src => new CashTransaction(src.Id));
            CreateMap<CostBaseAdjustmentTransactionItem, CostBaseAdjustment>()
                .ConstructUsing(src => new CostBaseAdjustment(src.Id));
            CreateMap<DisposalTransactionItem, Disposal>()
                .ConstructUsing(src => new Disposal(src.Id));
            CreateMap<IncomeTransactionItem, IncomeReceived>()
                .ConstructUsing(src => new IncomeReceived(src.Id));
            CreateMap<OpeningBalanceTransactionItem, OpeningBalance>()
                .ConstructUsing(src => new OpeningBalance(src.Id));
            CreateMap<ReturnOfCapitalTransactionItem, ReturnOfCapital>()
                .ConstructUsing(src => new ReturnOfCapital(src.Id));
            CreateMap<UnitCountAdjustmentTransactionItem, UnitCountAdjustment>()
                .ConstructUsing(src => new UnitCountAdjustment(src.Id));
        }
    }

    public class StockResolver : IValueResolver<Transaction, TransactionItem, StockItem>
    {
        private StockExchange _StockExchange;

        public StockResolver(StockExchange stockExchange)
        {
            _StockExchange = stockExchange;
        }

        public StockItem Resolve(Transaction source, TransactionItem destination, StockItem member, ResolutionContext context)
        {
            if (source.ASXCode == "")
                return new StockItem(Guid.Empty, "", "");

            try
            {
                return _StockExchange.Stocks.Get(source.ASXCode, source.TransactionDate).ToStockItem(source.TransactionDate);
            }
            catch
            {
                return new StockItem(Guid.Empty, source.ASXCode, "");
            }

        }
    }
}
