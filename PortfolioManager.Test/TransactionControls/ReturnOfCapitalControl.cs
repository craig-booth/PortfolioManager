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
    public partial class ReturnOfCapitalControl : UserControl, ITransactionControl
    {
        private StockService _StockService;

        public ReturnOfCapitalControl()
        {
            InitializeComponent();
        }

        public ReturnOfCapitalControl(StockService stockService)
            : this()
        {
            _StockService = stockService;

            dtpPaymentDate_ValueChanged(this, null);
        }

        public Transaction CreateTransaction()
        {
            var transaction = new ReturnOfCapital();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(Transaction transaction)
        {
            ReturnOfCapital returnOfCapital = transaction as ReturnOfCapital;

            dtpPaymentDate.Value = returnOfCapital.TransactionDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == returnOfCapital.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };

            txtAmount.Text = returnOfCapital.Amount.ToString("n");
            txtComment.Text = returnOfCapital.Comment;
        }

        public void UpdateTransaction(Transaction transaction)
        {
            ReturnOfCapital returnOfCapital = transaction as ReturnOfCapital;

            Stock stock = cboASXCode.SelectedItem as Stock;
            returnOfCapital.ASXCode = stock.ASXCode;
            returnOfCapital.TransactionDate = dtpPaymentDate.Value;
            returnOfCapital.Amount = MathUtils.ParseDecimal(txtAmount.Text);
            returnOfCapital.Comment = txtComment.Text;
        }

        private void dtpPaymentDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpPaymentDate.Value).Where(x => x.Type != StockType.StapledSecurity).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());
        }
    }
}
