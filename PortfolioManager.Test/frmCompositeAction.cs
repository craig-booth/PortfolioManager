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
    public partial class frmCompositeAction : Form, ICorporateActionForm
    {
        private Mode _Mode;
        private StockService _StockService;
        private CorporateActionService _CorporateActionService;
        private CompositeAction _CompositeAction;
        private Stock _Stock;

        public frmCompositeAction()
        {
            InitializeComponent();
        }

        public frmCompositeAction(StockService stockService, CorporateActionService corporateActionService)
            : this()
        {
            _StockService = stockService;
            _CorporateActionService = corporateActionService;
        }


        private void SetFormValues()
        {
           lblASXCode.Text = _StockService.GetASXCode(_CompositeAction.Stock);
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
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _CompositeAction.ActionDate = dtpActionDate.Value;
                _CompositeAction.Description = txtDescription.Text;

                _CorporateActionService.UpdateCorporateAction(_CompositeAction);
                                    
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
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _CorporateActionService.DeleteCorporateAction(_CompositeAction);
                return true;
            }
            return
                false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _CompositeAction = new CompositeAction(_Stock.Id, dtpActionDate.Value,
                                    txtDescription.Text);

                _CorporateActionService.AddCorporateAction(_CompositeAction);
                

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
