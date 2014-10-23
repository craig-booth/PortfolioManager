using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;

namespace PortfolioManager.Test
{
    public partial class frmAquisition : Form
    {
        private Mode _Mode;
        private StockManager _StockManager;
        private Aquisition  _Aquisition;

        public frmAquisition()
        {
            InitializeComponent();
        }

        public frmAquisition(StockManager stockManager, Aquisition aquisition, Mode mode)
            : this()
        {
            _Mode = mode;
            _StockManager = stockManager;
            _Aquisition = aquisition;

            dtpAquisitionDate_ValueChanged(this, null);
        }

        public static Aquisition AddAquisition(StockManager stockManager)
        {
            frmAquisition form = new frmAquisition(stockManager, null, Mode.Create);

            if (form.ShowDialog() == DialogResult.OK)
                return form._Aquisition;
            else
                return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                Stock stock = cboASXCode.SelectedItem as Stock;

                int units = 0;
                decimal averagePrice = 0.00M;
                decimal transactionCosts = 0.00M;

                if (txtUnits.Text != "")
                    units = int.Parse(txtUnits.Text);
                if (txtAveragePrice.Text != "")
                    averagePrice = decimal.Parse(txtAveragePrice.Text) / 100;
                if (txtTransactionCosts.Text != "")
                    transactionCosts = decimal.Parse(txtTransactionCosts.Text) / 100;

                _Aquisition = new Aquisition()
                {
                    ASXCode = stock.ASXCode,
                    TransactionDate = dtpAquisitionDate.Value,
                    Units = units,
                    AveragePrice = averagePrice,
                    TransactionCosts = transactionCosts,
                    Comment = txtComment.Text

                };

            }
        }

        private void dtpAquisitionDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockManager.GetStocks(dtpAquisitionDate.Value);

            cboASXCode.Items.Clear();
            foreach (Stock stock in stockList)
            {
                cboASXCode.Items.Add(stock);
            }
        }

    }
}
