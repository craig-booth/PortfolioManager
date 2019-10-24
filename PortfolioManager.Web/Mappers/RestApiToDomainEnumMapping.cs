using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioManager.Web.Mappers
{
    public static class RestApiToDomainEnumMapping
    {
        public static Domain.DRPMethod ToDomain(this RestApi.DRPMethod value)
        {
            return (Domain.DRPMethod)((int)value);
        }
        public static Domain.AssetCategory ToDomain(this RestApi.AssetCategory value)
        {
            return (Domain.AssetCategory)((int)value);
        }

        public static Domain.CGTMethod ToDomain(this RestApi.CGTMethod value)
        {
            return (Domain.CGTMethod)((int)value);
        }

        public static Domain.CGTCalculationMethod ToDomain(this RestApi.CGTCalculationMethod value)
        {
            return (Domain.CGTCalculationMethod)((int)value);
        }

        public static Domain.BankAccountTransactionType ToDomain(this RestApi.BankAccountTransactionType value)
        {
            return (Domain.BankAccountTransactionType)((int)value);
        }
        public static Domain.TransactionType ToDomain(this RestApi.TransactionType value)
        {
            return (Domain.TransactionType)((int)value);
        }
        public static Domain.CorporateActionType ToDomain(this RestApi.CorporateActionType value)
        {
            return (Domain.CorporateActionType)((int)value);
        }
    }
}
