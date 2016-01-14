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
    
    public partial class frmSplitConsolidation : Form, ICorporateActionForm
    {
        private Mode _Mode;
        private StockManager _StockManager;
        private SplitConsolidation _SplitConsolidation;
        private Stock _Stock;

        public frmSplitConsolidation()
        {
            InitializeComponent();
        }


        public frmSplitConsolidation(StockManager stockManager)
            : this()
        {
            _StockManager = stockManager;
        }


        private void SetFormValues()
        {
            lblASXCode.Text = _StockManager.GetASXCode(_SplitConsolidation.Stock);
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
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _SplitConsolidation = corporateAction as SplitConsolidation;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _SplitConsolidation.Change(dtpActionDate.Value,
                                    MathUtils.ParseInt(txtOriginalUnits.Text),
                                    MathUtils.ParseInt(txtNewUnits.Text),
                                    txtDescription.Text);

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
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _SplitConsolidation = corporateAction as SplitConsolidation;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _Stock.DeleteCorporateAction(_SplitConsolidation);
                return true;
            }
            return
                false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _SplitConsolidation = _Stock.AddSplitConsolidation(dtpActionDate.Value,
                                    MathUtils.ParseInt(txtOriginalUnits.Text),
                                    MathUtils.ParseInt(txtNewUnits.Text),
                                    txtDescription.Text);
            }
        }

    }

}
