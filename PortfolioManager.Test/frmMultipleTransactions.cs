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
using PortfolioManager.Model.Utils;
using PortfolioManager.Test.TransactionControls;

namespace PortfolioManager.Test
{
    public partial class frmMultipleTransactions : Form
    {
        private readonly StockService _StockService;

        public frmMultipleTransactions()
        {
            InitializeComponent();
        }

        public frmMultipleTransactions(StockService stockService)
            : this()
        {
            _StockService = stockService;
        }

        private void AddTransactionTab(ITransaction transaction)
        {
            UserControl control;
            string label;
            if (transaction.Type == TransactionType.Aquisition)
            {
                control = new AquisitionControl(_StockService);
                label = "Aquisition";
            }
            else if (transaction.Type == TransactionType.CostBaseAdjustment)
            {
                control = new CostBaseAdjustmentControl(_StockService);
                label = "Cost Base Adjustment";
            }
            else if (transaction.Type == TransactionType.Disposal)
            {
                control = new DisposalControl(_StockService);
                label = "Disposal";
            }
            else if (transaction.Type == TransactionType.Income)
            {
                control = new IncomeControl(_StockService);
                label = "Income";
            }
            else if (transaction.Type == TransactionType.OpeningBalance)
            {
                control = new OpeningBalanceControl(_StockService);
                label = "Opening Balance";
            }
            else if (transaction.Type == TransactionType.ReturnOfCapital)
            {
                control = new ReturnOfCapitalControl(_StockService);
                label = "Return Of Capital";
            }
            else if (transaction.Type == TransactionType.UnitCountAdjustment)
            {
                control = new UnitAdjustmentControl(_StockService);
                label = "Unit Count Adjustment";
            }
            else
                throw new NotSupportedException();

            var tabPage = new TabPage(label);
            tabTransactions.TabPages.Add(tabPage);
            tabPage.Controls.Add(control);
            tabPage.Tag = transaction;

            control.Visible = true;  
            (control as ITransactionControl).DisplayTransaction(transaction);
        }

        public bool EditTransactions(IEnumerable<ITransaction> transactions)
        {
            foreach (ITransaction transaction in transactions)
                AddTransactionTab(transaction);

            if (ShowDialog() == DialogResult.OK)
            {
                foreach (TabPage tabPage in tabTransactions.TabPages)
                {
                    ITransactionControl transactionControl = tabPage.Controls[0] as ITransactionControl;
                    ITransaction transaction = tabPage.Tag as ITransaction;

                    transactionControl.UpdateTransaction(transaction);
                }

                return true;
            }
            else
                return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }
    }
}
