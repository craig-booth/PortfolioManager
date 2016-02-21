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
           lblASXCode.Text = _StockManager.GetASXCode(_CompositeAction.Stock);
           dtpActionDate.Value = _CompositeAction.ActionDate;
           txtDescription.Text = _CompositeAction.Description;
        }

        public ICorporateAction CreateCorporateAction(Stock stock)
        {
            _Stock = stock;
            _Mode = Mode.Create;

            lblASXCode.Text = stock.ASXCode;

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
                _CompositeAction.Change(dtpActionDate.Value,
                                    txtDescription.Text);
                                    
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
                _StockManager.CorporateActionService.DeleteCorporateAction(_CompositeAction);
                return true;
            }
            return
                false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _CompositeAction = _Stock.AddCompositeAction(dtpActionDate.Value,
                                    txtDescription.Text);
                

            }
        }

        private void btnAddCapitalReturn_Click(object sender, EventArgs e)
        {

        }

        private void btnAddSplitConsolidation_Click(object sender, EventArgs e)
        {

        }

        private void btnAddTransformation_Click(object sender, EventArgs e)
        {

        }

        private void btnAddDividend_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteChildAction_Click(object sender, EventArgs e)
        {

        }
    }
}
