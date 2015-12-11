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
    public partial class frmCompositeAction : Form, ICorporateActionForm
    {
        private Mode _Mode;
        private StockManager _StockManager;
        private CompositeAction _CompositeAction;
        private Stock _Stock;

        public frmCompositeAction()
        {
            InitializeComponent();
        }

        public frmCompositeAction(StockManager stockManager)
            : this()
        {
            _StockManager = stockManager;
        }


        private void SetFormValues()
        {
           // lblASXCode.Text = _StockManager.GetASXCode(_CapitalReturn.Stock);
           // dtpRecordDate.Value = _CapitalReturn.ActionDate;
           // dtpPaymentDate.Value = _CapitalReturn.PaymentDate;
           // txtAmount.Text = MathUtils.FormatCurrency(_CapitalReturn.Amount, false);
           // txtDescription.Text = _CapitalReturn.Description;
        }

        public ICorporateAction CreateCorporateAction(Stock stock)
        {
            _Stock = stock;
            _Mode = Mode.Create;

          //  lblASXCode.Text = stock.ASXCode;

            if (ShowDialog() == DialogResult.OK)
            {
                return _CompositeAction;
            }
            else
                return null;
        }

        public bool EditCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
             /*   _CapitalReturn.Change(dtpRecordDate.Value,
                                    dtpPaymentDate.Value,
                                    MathUtils.ParseDecimal(txtAmount.Text),
                                    txtDescription.Text);
                                    */
                return true;
            }
            else
                return false;
        }

        public void ViewCorporateAction(ICorporateAction corporateAction)
        {
            _Mode = Mode.View;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            ShowDialog();
        }

        public Boolean DeleteCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _Stock.DeleteCorporateAction(_CompositeAction);
                return true;
            }
            return
                false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
             /*   _CapitalReturn = _Stock.AddCapitalReturn(dtpRecordDate.Value,
                                    dtpPaymentDate.Value,
                                    MathUtils.ParseDecimal(txtAmount.Text),
                                    txtDescription.Text); */
            }
        }


    }
}
