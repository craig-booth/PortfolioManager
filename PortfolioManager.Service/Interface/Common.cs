using System;

using PortfolioManager.Common;
using PortfolioManager.Data.Stocks;

namespace PortfolioManager.Service.Interface
{

    public class StockItem
    {
        public Guid Id { get; set; }
        public string ASXCode { get; set; }
        public string Name { get; set; }

        public StockItem()
        {

        }

        public StockItem(Guid id, string asxCode, string name)
        {
            Id = id;
            ASXCode = asxCode;
            Name = name;
        } 

        internal StockItem(Stock stock)
        {
            Id = stock.Id;
            ASXCode = stock.ASXCode;
            Name = stock.Name;
        }
    }

    public class HoldingItem
    {
        public StockItem Stock;
        public AssetCategory Category;
        public int Units;
        public decimal Value;
        public decimal Cost;
    }

    public enum ResponceStatus { Failed, Successfull }

    public class ServiceResponce
    {
        public DateTime ResponceTime { get; set; }
        public ResponceStatus Status { get; set; }
        public string Error { get; set; }

        public ServiceResponce()
        {
            ResponceTime = DateTime.Now;
            Status = ResponceStatus.Failed;
            Error = "Not initialised";
        }

        public void SetStatusToSuccessfull()
        {
            Status = ResponceStatus.Successfull;
            Error = "";
        }

        public void SetStatusToFailed(string errorMessage)
        {
            Status = ResponceStatus.Failed;
            Error = errorMessage;
        }
    }
}
