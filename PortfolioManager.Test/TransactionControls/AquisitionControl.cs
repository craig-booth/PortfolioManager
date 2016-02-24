using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Utils;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Test.TransactionControls
{
    public partial class AquisitionControl : UserControl, ITransactionControl 
    {
        private readonly StockService _StockService;

        public AquisitionControl()
        {
            InitializeComponent();
        }

        public AquisitionControl(StockService stockService)
            : this()
        {
            _StockService = stockService;

            dtpAquisitionDate_ValueChanged(this, null);
        }

        public ITransaction CreateTransaction()
        {
            var transaction = new Aquisition();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(ITransaction transaction)
        {
            Aquisition aquisition = transaction as Aquisition;

            dtpAquisitionDate.Value = aquisition.TransactionDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == aquisition.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };

            txtUnits.Text = aquisition.Units.ToString();
            txtAveragePrice.Text = MathUtils.FormatCurrency(aquisition.AveragePrice);
            txtTransactionCosts.Text = MathUtils.FormatCurrency(aquisition.TransactionCosts);
            txtComment.Text = aquisition.Comment;
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            Aquisition aquisition = transaction as Aquisition;

            Stock stock = cboASXCode.SelectedItem as Stock;
            aquisition.ASXCode = stock.ASXCode;
            aquisition.TransactionDate = dtpAquisitionDate.Value;
            aquisition.Units = MathUtils.ParseInt(txtUnits.Text);
            aquisition.AveragePrice = MathUtils.ParseDecimal(txtAveragePrice.Text);
            aquisition.TransactionCosts = MathUtils.ParseDecimal(txtTransactionCosts.Text);
            aquisition.Comment = txtComment.Text;
        }

        private void dtpAquisitionDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpAquisitionDate.Value).Where(x => x.ParentId == Guid.Empty).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());
        }
    }
}
