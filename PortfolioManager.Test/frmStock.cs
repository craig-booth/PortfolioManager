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
using StockManager.Service;

namespace PortfolioManager.Test
{

    public partial class frmStock : Form
    {
        private Mode _Mode;
        private StockService _StockService;

        public frmStock()
        {
            InitializeComponent();
        }

        public frmStock(StockService stockService, Stock stock, Mode mode)
            : this()
        {
            _Mode = mode;
            _StockService = stockService;
        }

        public static DialogResult EditStock(StockService stockService, Stock stock)
        {
            frmStock form = new frmStock(stockService, stock, Mode.Edit);

            return form.ShowDialog();
        }

        public static DialogResult AddStock(StockService stockService)
        {
            frmStock form = new frmStock(stockService, null, Mode.Create);

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
                    fromDate = DateTimeConstants.NoStartDate;
                else
                    fromDate = dtpFromDate.Value;
                _StockService.Add(txtASXCode.Text, txtName.Text, fromDate, stockType);
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
