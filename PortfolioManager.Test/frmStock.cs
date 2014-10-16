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
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Test
{

    public partial class frmStock : Form
    {
        private Mode _Mode;
        private StockManager _StockManager;

        public frmStock()
        {
            InitializeComponent();
        }

        public frmStock(StockManager stockManager, Stock stock, Mode mode)
            : this()
        {
            _Mode = mode;
            _StockManager = stockManager;
        }

        public static DialogResult EditStock(StockManager stockManager, Stock stock)
        {
            frmStock form = new frmStock(stockManager, stock, Mode.Edit);

            return form.ShowDialog();
        }

        public static DialogResult AddStock(StockManager stockManager)
        {
            frmStock form = new frmStock(stockManager, null, Mode.Create);

            form.cboType.SelectedIndex = 0;
            form.chkNoStartDate.Checked = true;
            form.chkNoEndDate.Checked = true;

            return form.ShowDialog();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                StockType stockType = (StockType) cboType.SelectedIndex;
                DateTime fromDate;
                if (chkNoStartDate.Checked)
                    fromDate = DateTimeConstants.NoStartDate();
                else
                    fromDate = dtpFromDate.Value;
                _StockManager.AddStock(txtASXCode.Text, txtName.Text, fromDate, stockType);
            }
        }

        private void chkNoStartDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpFromDate.Enabled = ! chkNoStartDate.Checked;
        }

        private void chkNoEndDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpToDate.Enabled = !chkNoEndDate.Checked;
        }



    }
}
