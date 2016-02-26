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
using PortfolioManager.Test.TransactionControls;
using PortfolioManager.Service;

namespace PortfolioManager.Test
{
    public partial class frmTransaction : Form
    {
        private readonly StockService _StockService;
        private ITransactionControl _TransactionControl;

        public frmTransaction()
        {
            InitializeComponent();
        }

        public frmTransaction(StockService stockService)
            : this()
        {
            _StockService = stockService;
        }

        private void ShowControl(TransactionType type)
        {
            UserControl control;
            if (type == TransactionType.Aquisition)
                control = new AquisitionControl(_StockService);
            else if (type == TransactionType.CostBaseAdjustment)
                control = new CostBaseAdjustmentControl(_StockService);
            else if (type == TransactionType.Disposal)
                control = new DisposalControl(_StockService);
            else if (type == TransactionType.Income)
                control = new IncomeControl(_StockService);
            else if (type == TransactionType.OpeningBalance)
                control = new OpeningBalanceControl(_StockService);
            else if (type == TransactionType.ReturnOfCapital)
                control = new ReturnOfCapitalControl(_StockService);
            else if (type == TransactionType.UnitCountAdjustment)
                control = new UnitAdjustmentControl(_StockService);
            else
                throw new NotSupportedException();

            _TransactionControl = control as ITransactionControl;

            control.Visible = true;
            pnlTransaction.Controls.Add(control);
        }

        public ITransaction CreateTransaction(TransactionType type)
        {
            ShowControl(type);

            if (ShowDialog() == DialogResult.OK)
                return _TransactionControl.CreateTransaction();
            else 
                return null;
        }

        public bool EditTransaction(ITransaction transaction)
        {
            ShowControl(transaction.Type);

            _TransactionControl.DisplayTransaction(transaction);
            if (ShowDialog() == DialogResult.OK)
            {
                _TransactionControl.UpdateTransaction(transaction);
                return true;
            }
            else
                return false;
        }

        public void ViewTransaction(ITransaction transaction)
        {
            ShowControl(transaction.Type);

            _TransactionControl.DisplayTransaction(transaction);
            ShowDialog(); 
        }

        public bool DeleteTransaction(ITransaction transaction)
        {
            ShowControl(transaction.Type);

            _TransactionControl.DisplayTransaction(transaction);
            return (ShowDialog() == DialogResult.OK); 
        }
    }

}
