using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortfolioManager.Test
{
    public partial class frmSettings : Form
    {
        public PortfolioManagerSettings Settings { get; private set; }

        public frmSettings()
        {
            InitializeComponent();
        }

        public frmSettings(PortfolioManagerSettings settings)
            : this()
        {
            Settings = settings;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings.PortfolioDatabaseFile = txtPortfolioDatabase.Text;
            Settings.StockDatabaseFile = txtStockDatabase.Text;
            Settings.Save();
        }

        private void btnBrowsePortfolioDatabase_Click(object sender, EventArgs e)
        {
            if (txtPortfolioDatabase.Text != "")
                openFileDialog.FileName = txtPortfolioDatabase.Text;
            else
                openFileDialog.FileName = "Portfolio.db";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtPortfolioDatabase.Text = openFileDialog.FileName;
            }
        }

        private void btnBrowseStockDatabase_Click(object sender, EventArgs e)
        {
            if (txtStockDatabase.Text != "")
                openFileDialog.FileName = txtStockDatabase.Text;
            else
                openFileDialog.FileName = "Stocks.db";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtStockDatabase.Text = openFileDialog.FileName;
            }
        }
    }
}
