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
        private CorporateActionFormFactory _CorporateActionFormFactory;
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

            _CorporateActionFormFactory = new CorporateActionFormFactory(stockService, corporateActionService);
        }


        private void SetFormValues()
        {
            lblASXCode.Text = _StockService.GetASXCode(_CompositeAction.Stock);
            dtpActionDate.Value = _CompositeAction.ActionDate;
            txtDescription.Text = _CompositeAction.Description;

            lsvChildActions.Items.Clear();
            foreach (var childAction in _CompositeAction.Children)
                AddChildAction(childAction);
        }

        private void AddChildAction(ICorporateAction childAction)
        {
            var item = lsvChildActions.Items.Add(childAction.Description);
            item.Tag = childAction;
        }

        private void UpdateChildAction(ICorporateAction childAction)
        {
            foreach (ListViewItem item in lsvChildActions.Items)
            {
                var itemAction = item.Tag as ICorporateAction; 
                if (childAction.Id == itemAction.Id)
                    item.Name = childAction.Description;
            }
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

                throw new Exception("Child Actions not supported");

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

                throw new Exception("Child Actions not supported");

                _CorporateActionService.AddCorporateAction(_CompositeAction);
                

            }
        }

        private void btnAddCapitalReturn_Click(object sender, EventArgs e)
        {
            var form = _CorporateActionFormFactory.CreateCorporateActionForm(CorporateActionType.CapitalReturn);
            var childAction = form.CreateCorporateAction(_Stock);

            if (childAction != null)
                AddChildAction(childAction);
        }
    
        private void btnAddSplitConsolidation_Click(object sender, EventArgs e)
        {
            var form = _CorporateActionFormFactory.CreateCorporateActionForm(CorporateActionType.SplitConsolidation);
            var childAction = form.CreateCorporateAction(_Stock);

            if (childAction != null)
                AddChildAction(childAction);
        }

        private void btnAddTransformation_Click(object sender, EventArgs e)
        {
            var form = _CorporateActionFormFactory.CreateCorporateActionForm(CorporateActionType.Transformation);
            var childAction = form.CreateCorporateAction(_Stock);

            if (childAction != null)
                AddChildAction(childAction);
        }

        private void btnAddDividend_Click(object sender, EventArgs e)
        {
            var form = _CorporateActionFormFactory.CreateCorporateActionForm(CorporateActionType.Dividend);
            var childAction = form.CreateCorporateAction(_Stock);

            if (childAction != null)
                AddChildAction(childAction);
        }

        private void btnDeleteChildAction_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lsvChildActions.SelectedItems)
                lsvChildActions.Items.Remove(item);
        }

        private void lsvChildActions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ICorporateAction childAction = lsvChildActions.FocusedItem.Tag as ICorporateAction;

            var form = _CorporateActionFormFactory.CreateCorporateActionForm(childAction.Type);
            if (form.EditCorporateAction(childAction))
            {
                UpdateChildAction(childAction);
            }
        }
    }
}
