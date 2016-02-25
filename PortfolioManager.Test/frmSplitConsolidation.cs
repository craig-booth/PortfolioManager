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
    
    public partial class frmSplitConsolidation : Form, ICorporateActionForm
    {
        private Mode _Mode;
        private StockService _StockService;
        private CorporateActionService _CorporateActionService;
        private SplitConsolidation _SplitConsolidation;
        private Stock _Stock;

        public frmSplitConsolidation()
        {
            InitializeComponent();
        }


        public frmSplitConsolidation(StockService stockService, CorporateActionService corporateActionService)
            : this()
        {
            _StockService = stockService;
            _CorporateActionService = corporateActionService;
        }


        private void SetFormValues()
        {
            lblASXCode.Text = _StockService.GetASXCode(_SplitConsolidation.Stock);
            dtpActionDate.Value = _SplitConsolidation.ActionDate;
            txtOriginalUnits.Text = _SplitConsolidation.OldUnits.ToString();
            txtNewUnits.Text = _SplitConsolidation.NewUnits.ToString();
            txtDescription.Text = _SplitConsolidation.Description;
        }

        public ICorporateAction CreateCorporateAction(Stock stock)
        {
            _Stock = stock;
            _Mode = Mode.Create;

            lblASXCode.Text = stock.ASXCode;

            if (ShowDialog() == DialogResult.OK)
            {
                return _SplitConsolidation;
            }
            else
                return null;
        }

        public bool EditCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _SplitConsolidation = corporateAction as SplitConsolidation;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _SplitConsolidation.ActionDate = dtpActionDate.Value;
                _SplitConsolidation.OldUnits = MathUtils.ParseInt(txtOriginalUnits.Text);
                _SplitConsolidation.NewUnits = MathUtils.ParseInt(txtNewUnits.Text);
                _SplitConsolidation.Description = txtDescription.Text;

                _CorporateActionService.UpdateCorporateAction(_SplitConsolidation);

                return true;
            }
            else
                return false;
        }

        public void ViewCorporateAction(ICorporateAction corporateAction)
        {
            _Mode = Mode.View;
            _SplitConsolidation = corporateAction as SplitConsolidation;
            SetFormValues();
            ShowDialog();
        }

        public Boolean DeleteCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _SplitConsolidation = corporateAction as SplitConsolidation;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _CorporateActionService.DeleteCorporateAction(_SplitConsolidation);
                return true;
            }
            return
                false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _SplitConsolidation = new SplitConsolidation(_Stock.Id, dtpActionDate.Value,
                                    MathUtils.ParseInt(txtOriginalUnits.Text),
                                    MathUtils.ParseInt(txtNewUnits.Text),
                                    txtDescription.Text);
                _CorporateActionService.AddCorporateAction(_SplitConsolidation);
            }
        }

    }

}
