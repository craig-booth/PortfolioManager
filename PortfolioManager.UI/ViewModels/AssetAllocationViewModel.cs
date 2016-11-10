using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using PortfolioManager.UI.Utilities;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.UI.ViewModels
{
    class AssetAllocationViewModel : PortfolioViewModel 
    {

        public ObservableCollection<CategoryValue> Categories { get; private set; }

        public AssetAllocationViewModel(string label, ViewParameter parameter)
            : base(label, parameter)
        {
            Options.AllowStockSelection = false;
            Options.DateSelection = DateSelectionType.Single;

            Categories = new ObservableCollection<CategoryValue>();
        }

        public override void RefreshView()
        {
            var holdings = _Parameter.Portfolio.ShareHoldingService.GetHoldings(_Parameter.Date);

            var cashBalance = _Parameter.Portfolio.CashAccountService.GetBalance(_Parameter.Date);

            var totalValue = holdings.Sum(x => x.MarketValue) + cashBalance;

            var holdingCategories = holdings.GroupBy(x => x.Stock.Category);

            Categories.Clear();
            foreach (var category in holdingCategories)
            {
                var value = category.Sum(x => x.MarketValue);
                Categories.Add(new CategoryValue(category.Key, value, value / totalValue));
            }
           
            Categories.Add(new ViewModels.CategoryValue(AssetCategory.Cash, cashBalance, cashBalance / totalValue));
        }
    }


    class CategoryValue
    {
        public AssetCategory Category { get; private set; }
        public decimal Value { get; set; }
        public decimal Percentage { get; set; }

        public CategoryValue(AssetCategory category, decimal value, decimal percentage)
        {
            Category = category;
            Value = value;
            Percentage = percentage;
        }
    }
}
