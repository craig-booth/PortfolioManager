using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioManager.Web.Mappers
{
    public static class DomainToRestApiEnumMapping
    {
        public static RestApi.DRPMethod ToRestApi(this Domain.DRPMethod value)
        {
            return (RestApi.DRPMethod)((int)value);
        }
        public static RestApi.AssetCategory ToRestApi(this Domain.AssetCategory value)
        {
            return (RestApi.AssetCategory)((int)value);
        }

        public static RestApi.CGTMethod ToRestApi(this Domain.CGTMethod value)
        {
            return (RestApi.CGTMethod)((int)value);
        }

        public static RestApi.CGTCalculationMethod ToRestApi(this Domain.CGTCalculationMethod value)
        {
            return (RestApi.CGTCalculationMethod)((int)value);
        }

        public static RestApi.BankAccountTransactionType ToRestApi(this Domain.BankAccountTransactionType value)
        {
            return (RestApi.BankAccountTransactionType)((int)value);
        }
        public static RestApi.TransactionType ToRestApi(this Domain.TransactionType value)
        {
            return (RestApi.TransactionType)((int)value);
        }
        public static RestApi.CorporateActionType ToRestApi(this Domain.CorporateActionType value)
        {
            return (RestApi.CorporateActionType)((int)value);
        }

    }
}
