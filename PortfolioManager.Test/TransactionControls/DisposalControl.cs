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
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Test.TransactionControls
{
    public partial class DisposalControl : UserControl, ITransactionControl 
    {
        private readonly StockService _StockService;

        public DisposalControl()
        {
            InitializeComponent();
        }
        public DisposalControl(StockService stockService)
            : this()
        {
            _StockService = stockService;
        }

        public ITransaction CreateTransaction()
        {
            var transaction = new Disposal();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(ITransaction transaction)
        {
            Disposal disposal = transaction as Disposal;

            dtpDisposalDate.Value = disposal.TransactionDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == disposal.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };

            txtUnits.Text = disposal.Units.ToString();
            txtAveragePrice.Text = MathUtils.FormatCurrency(disposal.AveragePrice);
            txtTransactionCosts.Text = MathUtils.FormatCurrency(disposal.TransactionCosts);
            txtComment.Text = disposal.Comment;
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            Disposal disposal = transaction as Disposal;

            Stock stock = cboASXCode.SelectedItem as Stock;
            disposal.ASXCode = stock.ASXCode;
            disposal.TransactionDate = dtpDisposalDate.Value;
            disposal.Units = MathUtils.ParseInt(txtUnits.Text);
            disposal.AveragePrice = MathUtils.ParseDecimal(txtAveragePrice.Text);
            disposal.TransactionCosts = MathUtils.ParseDecimal(txtTransactionCosts.Text);
            disposal.Comment = txtComment.Text;
        }

        private void dtpDisposalDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpDisposalDate.Value).Where(x => x.ParentId == Guid.Empty).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());        
        }
    }
}






