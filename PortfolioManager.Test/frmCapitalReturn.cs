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
    public partial class frmCapitalReturn : Form, ICorporateActionForm 
    {
        private Mode _Mode;
        private StockManager _StockManager;
        private CapitalReturn _CapitalReturn;
        private Stock _Stock;

        public frmCapitalReturn()
        {
            InitializeComponent();
        }


        public frmCapitalReturn(StockManager stockManager)
            : this()
        {
            _StockManager = stockManager;
        }
        

        private void SetFormValues()
        {
            lblASXCode.Text = _StockManager.GetASXCode(_CapitalReturn.Stock);
            dtpRecordDate.Value = _CapitalReturn.ActionDate;
            dtpPaymentDate.Value = _CapitalReturn.PaymentDate;
            txtAmount.Text = MathUtils.FormatCurrency(_CapitalReturn.Amount, false);
            txtDescription.Text = _CapitalReturn.Description;
        } 

        public ICorporateAction CreateCorporateAction(Stock stock)
        {
            _Stock = stock;
            _Mode = Mode.Create;

            lblASXCode.Text = stock.ASXCode;

            if (ShowDialog() == DialogResult.OK)
            {
                return _CapitalReturn;
            }
            else
                return null;
        }

        public bool EditCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _CapitalReturn = corporateAction as CapitalReturn;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _CapitalReturn.Change(dtpRecordDate.Value,
                                    dtpPaymentDate.Value,
                                    MathUtils.ParseDecimal(txtAmount.Text),
                                    txtDescription.Text);

                return true;
            }
            else
                return false;
        } 

        public void ViewCorporateAction(ICorporateAction corporateAction)
        {
            _Mode = Mode.View;
            _CapitalReturn = corporateAction as CapitalReturn;
            SetFormValues();
            ShowDialog();
        } 

        public Boolean DeleteCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockManager.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _CapitalReturn = corporateAction as CapitalReturn;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _Stock.DeleteCorporateAction(_CapitalReturn);
                return true;
            }
            return
                false;
        } 

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _CapitalReturn = _Stock.AddCapitalReturn(dtpRecordDate.Value,
                                    dtpPaymentDate.Value,
                                    MathUtils.ParseDecimal(txtAmount.Text),
                                    txtDescription.Text);
            }
        }

    }
}
