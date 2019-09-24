using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PortfolioManager.Web
{
    public class IdNotUniqueException : Exception { }

    public class StockAlreadyExistsException : Exception
    {
        string AsxCode { get; }
        DateTime ListingDate  { get; }

        public StockAlreadyExistsException(string asxCode, DateTime listingDate)
        {
            AsxCode = asxCode;
            ListingDate = listingDate;
        }
    }

    public class EntityNotFoundException : Exception
    {
        Guid Id { get; }

        public EntityNotFoundException(Guid id)
        {
            Id = id;
        }
    }

    public class PortfolioNotFoundException : EntityNotFoundException { public PortfolioNotFoundException(Guid id) : base(id) { } }
    public class HoldingNotFoundException : EntityNotFoundException { public HoldingNotFoundException(Guid id) : base(id) { } }
    public class CorporateActionNotFoundException : EntityNotFoundException { public CorporateActionNotFoundException(Guid id) : base(id) { } }
    public class TransactionNotFoundException : EntityNotFoundException { public TransactionNotFoundException(Guid id) : base(id) { } }
    public class StockNotFoundException : EntityNotFoundException { public StockNotFoundException(Guid id) : base(id) { } }

    public class UnknownCorporateActionType : Exception { }

    public class PortfolioExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is EntityNotFoundException)
                context.Result = new NotFoundResult();
        }
    }

}
