using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Test
{
    public partial class frmDividend : Form
    {
        private Mode _Mode;
        private StockManager _StockManager;

        public frmDividend()
        {
            InitializeComponent();
        }

        public frmDividend(StockManager stockManager, Dividend dividend, Mode mode)
            : this()
        {
            _Mode = mode;
            _StockManager = stockManager;

            dtpRecordDate_ValueChanged(this, null);
        }

        public static DialogResult EditDividend(StockManager stockManager, Dividend dividend)
        {
            frmDividend form = new frmDividend(stockManager, dividend, Mode.Edit);

            return form.ShowDialog();
        }

        public static DialogResult AddDividend(StockManager stockManager)
        {
            frmDividend form = new frmDividend(stockManager, null, Mode.Create);

            form.txtCompanyTaxRate.Text = "30";

            return form.ShowDialog();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                Stock stock = cboASXCode.SelectedItem as Stock;
                
                decimal percentFranked = 0.00M;
                decimal companyTaxRate = 0.30M;
                decimal drpPrice = 0.00M;
                decimal amount = decimal.Parse(txtDividendAmount.Text);
                if (txtPercentFranked.Text != "")
                    percentFranked = decimal.Parse(txtPercentFranked.Text) / 100;              
                if (txtCompanyTaxRate.Text != "")
                    companyTaxRate = decimal.Parse(txtCompanyTaxRate.Text) / 100;
                if (txtDRPPrice.Text != "")
                    drpPrice = decimal.Parse(txtDRPPrice.Text);

                stock.AddDividend(dtpRecordDate.Value, dtpPaymentDate.Value, amount, percentFranked, companyTaxRate, drpPrice, "");

            }
        }

        private void dtpRecordDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockManager.GetStocks(dtpRecordDate.Value);

            cboASXCode.Items.Clear();
            foreach (Stock stock in stockList)
            {
                cboASXCode.Items.Add(stock);
            }
            

        }

    }
}
