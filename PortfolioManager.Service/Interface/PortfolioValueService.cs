﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PortfolioManager.Common;

namespace PortfolioManager.Service.Interface
{
    public interface IPortfolioValueService
    {
        Task<PortfolioValueResponce> GetPortfolioValue(DateTime fromDate, DateTime toDate, ValueFrequency frequency);
        Task<PortfolioValueResponce> GetPortfolioValue(Guid stockId, DateTime fromDate, DateTime toDate, ValueFrequency frequency);
    }

    public enum ValueFrequency { Daily, Weekly, Monthly };

    public class PortfolioValueResponce  : ServiceResponce
    {
        public List<DailyAmount> Values;

        public PortfolioValueResponce()
            : base()
        {
            Values = new List<DailyAmount>();
        }
    }
}
