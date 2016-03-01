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
using PortfolioManager.Service;

namespace PortfolioManager.Test.TransactionControls
{
    public partial class OpeningBalanceControl : UserControl, ITransactionControl
    {

        private StockService _StockService;
        private bool _AquisitionDateSet;

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
            _AquisitionDateSet = false;
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
            dtpAquisitionDate.Value = openingBalance.AquisitionDate;
            txtComment.Text = openingBalance.Comment;

            _AquisitionDateSet = true;
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            OpeningBalance openingBalance = transaction as OpeningBalance;

            Stock stock = cboASXCode.SelectedItem as Stock;
            openingBalance.ASXCode = stock.ASXCode;
            openingBalance.TransactionDate = dtpBalanceDate.Value;
            openingBalance.Units = MathUtils.ParseInt(txtUnits.Text);
            openingBalance.CostBase = MathUtils.ParseDecimal(txtCostBase.Text);
            openingBalance.AquisitionDate = dtpAquisitionDate.Value;
            openingBalance.Comment = txtComment.Text;
        }

        private void dtpBalanceDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpBalanceDate.Value).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());

            if (!_AquisitionDateSet)
                dtpAquisitionDate.Value = dtpBalanceDate.Value;
        }
    }
}


