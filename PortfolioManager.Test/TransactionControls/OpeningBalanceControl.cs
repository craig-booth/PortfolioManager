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
    public partial class OpeningBalanceControl : UserControl, ITransactionControl
    {

        private StockService _StockService;

        public OpeningBalanceControl()
        {
            InitializeComponent();
        }

        public OpeningBalanceControl(StockService stockService)
            : this()
        {
            _StockService = stockService;

            dtpBalanceDate_ValueChanged(this, null);
        }

        public ITransaction CreateTransaction()
        {
            var transaction = new OpeningBalance();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(ITransaction transaction)
        {
            OpeningBalance openingBalance = transaction as OpeningBalance;

            dtpBalanceDate.Value = openingBalance.TransactionDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == openingBalance.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };

            txtUnits.Text = openingBalance.Units.ToString();
            txtCostBase.Text = openingBalance.CostBase.ToString("n");
            txtComment.Text = openingBalance.Comment;
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            OpeningBalance openingBalance = transaction as OpeningBalance;

            Stock stock = cboASXCode.SelectedItem as Stock;
            openingBalance.ASXCode = stock.ASXCode;
            openingBalance.TransactionDate = dtpBalanceDate.Value;
            openingBalance.Units = MathUtils.ParseInt(txtUnits.Text);
            openingBalance.CostBase = MathUtils.ParseDecimal(txtCostBase.Text);
            openingBalance.Comment = txtComment.Text;
        }

        private void dtpBalanceDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpBalanceDate.Value).Where(x => x.ParentId == Guid.Empty).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());
        }
    }
}


