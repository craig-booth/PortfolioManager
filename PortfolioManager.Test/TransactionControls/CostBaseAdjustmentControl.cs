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
using PortfolioManager.Service;

namespace PortfolioManager.Test.TransactionControls
{
    public partial class CostBaseAdjustmentControl : UserControl, ITransactionControl 
    {
        private StockService _StockService;

        public CostBaseAdjustmentControl()
        {
            InitializeComponent();
        }

        public CostBaseAdjustmentControl(StockService stockService)
            : this()
        {
            _StockService = stockService;

            dtpAdjustmentDate_ValueChanged(this, null);
        }

        public Transaction CreateTransaction()
        {
            var transaction = new CostBaseAdjustment();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(Transaction transaction)
        {
            CostBaseAdjustment costbaseAdjustment = transaction as CostBaseAdjustment;

            dtpAdjustmentDate.Value = costbaseAdjustment.TransactionDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == costbaseAdjustment.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };

            txtPercentage.Text = (costbaseAdjustment.Percentage * 100).ToString("#0.##");
            txtComment.Text = costbaseAdjustment.Comment;
        }

        public void UpdateTransaction(Transaction transaction)
        {
            CostBaseAdjustment costbaseAdjustment = transaction as CostBaseAdjustment;

            Stock stock = cboASXCode.SelectedItem as Stock;
            costbaseAdjustment.ASXCode = stock.ASXCode;
            costbaseAdjustment.TransactionDate = dtpAdjustmentDate.Value;
            costbaseAdjustment.Percentage = MathUtils.ParseDecimal(txtPercentage.Text) / 100;      
            costbaseAdjustment.Comment = txtComment.Text;
        }

        private void dtpAdjustmentDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpAdjustmentDate.Value).Where(x => x.Type != StockType.StapledSecurity).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());
        }

    }
}

