using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioValueService : IPortfolioService
    {
        Task<PortfolioValueResponce> GetPortfolioValue(DateTime fromDate, DateTime toDate, ValueFrequency frequency);
        Task<PortfolioValueResponce> GetPortfolioValue(Guid stockId, DateTime fromDate, DateTime toDate, ValueFrequency frequency);
    }

    public enum ValueFrequency { Daily, Weekly, Monthly };

    public class PortfolioValueResponce  : ServiceResponce
    {
        public Dictionary<DateTime, decimal> Values;

        public PortfolioValueResponce()
            : base()
        {
            Values = new Dictionary<DateTime, decimal>();
        }
    }
}
