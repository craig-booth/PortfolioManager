using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioValueService : IPortfolioManagerService
    {
        Task<PortfolioValueResponce> GetPortfolioValue(DateTime fromDate, DateTime toDate, ValueFrequency frequency);
        Task<PortfolioValueResponce> GetPortfolioValue(Guid stockId, DateTime fromDate, DateTime toDate, ValueFrequency frequency);
    }

    public enum ValueFrequency { Daily, Weekly, Monthly };

    public class PortfolioValueResponce
    {
        public Dictionary<DateTime, decimal> Values;

        public PortfolioValueResponce()
        {
            Values = new Dictionary<DateTime, decimal>();
        }
    }
}
