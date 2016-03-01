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
    public partial class UnitAdjustmentControl : UserControl, ITransactionControl
    {
        private StockService _StockService;

        public UnitAdjustmentControl()
        {
            InitializeComponent();
        }

        public UnitAdjustmentControl(StockService stockService)
            : this()
        {
            _StockService = stockService;

            dtpAdjustmentDate_ValueChanged(this, null);
        }

        public ITransaction CreateTransaction()
        {
            var transaction = new UnitCountAdjustment();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(ITransaction transaction)
        {
            UnitCountAdjustment unitCountAdjustment = transaction as UnitCountAdjustment;

            dtpAdjustmentDate.Value = unitCountAdjustment.TransactionDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == unitCountAdjustment.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };
        
            txtOldUnits.Text = unitCountAdjustment.OriginalUnits.ToString("#0");
            txtNewUnits.Text = unitCountAdjustment.NewUnits.ToString("#0");
            txtComment.Text = unitCountAdjustment.Comment;
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            UnitCountAdjustment unitCountAdjustment = transaction as UnitCountAdjustment;

            Stock stock = cboASXCode.SelectedItem as Stock;
            unitCountAdjustment.ASXCode = stock.ASXCode;
            unitCountAdjustment.TransactionDate = dtpAdjustmentDate.Value;
            unitCountAdjustment.OriginalUnits = MathUtils.ParseInt(txtOldUnits.Text);
            unitCountAdjustment.NewUnits = MathUtils.ParseInt(txtNewUnits.Text);
            unitCountAdjustment.Comment = txtComment.Text;
        }

        private void dtpAdjustmentDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpAdjustmentDate.Value).Where(x => x.Type != StockType.StapledSecurity).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());
        }


    }
}
