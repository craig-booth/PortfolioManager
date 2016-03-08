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
        private CompositeAction _CompositeAction;
        private CorporateActionFormFactory _CorporateActionFormFactory;
        private Stock _Stock;

        public frmCompositeAction()
        {
            InitializeComponent();
        }

        public frmCompositeAction(StockService stockService)
            : this()
        {
            _StockService = stockService;

            _CorporateActionFormFactory = new CorporateActionFormFactory(stockService);
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

        private void AddChildAction(CorporateAction childAction)
        {
            var item = lsvChildActions.Items.Add(childAction.Type.ToString());
            item.SubItems.Add(childAction.Description);
            item.Tag = childAction;
        }

        private void UpdateChildAction(CorporateAction childAction)
        {
            foreach (ListViewItem item in lsvChildActions.Items)
            {
                var itemAction = item.Tag as CorporateAction; 
                if (childAction.Id == itemAction.Id)
                    item.Name = childAction.Description;
            }
        }

        public CorporateAction CreateCorporateAction(Stock stock)
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

        public bool EditCorporateAction(CorporateAction corporateAction)
        {
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _CompositeAction.ActionDate = dtpActionDate.Value;
                _CompositeAction.Description = txtDescription.Text;

                _CompositeAction.Children.Clear();
                foreach (ListViewItem item in lsvChildActions.Items)
                {
                    var childAction = item.Tag as CorporateAction;
                    _CompositeAction.Children.Add(childAction);
                }
                                                    
                return true;
            }
            else
                return false;
        }

        public void ViewCorporateAction(CorporateAction corporateAction)
        {
            _Mode = Mode.View;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            ShowDialog();
        }

        public Boolean DeleteCorporateAction(CorporateAction corporateAction)
        {
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _CompositeAction = corporateAction as CompositeAction;
            SetFormValues();
            return (ShowDialog() == DialogResult.OK);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _CompositeAction = new CompositeAction(_Stock.Id, dtpActionDate.Value,
                                    txtDescription.Text);

                foreach (ListViewItem item in lsvChildActions.Items)
                {
                    var childAction = item.Tag as CorporateAction;
                    _CompositeAction.Children.Add(childAction);
                }
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
            var childAction = lsvChildActions.FocusedItem.Tag as CorporateAction;

            var form = _CorporateActionFormFactory.CreateCorporateActionForm(childAction.Type);
            if (form.EditCorporateAction(childAction))
            {
                UpdateChildAction(childAction);
            }
        }
    }
}
