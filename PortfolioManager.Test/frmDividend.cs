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
    public partial class frmDividend : Form, ICorporateActionForm 
    {
        private Mode _Mode;
        private StockService _StockService;
        private CorporateActionService _CorporateActionService;
        private Dividend _Dividend;
        private Stock _Stock;

        public frmDividend()
        {
            InitializeComponent();
        }

        public frmDividend(StockService stockService, CorporateActionService corporateActionService)
            : this()
        {
            _StockService = stockService;
            _CorporateActionService = corporateActionService;
        }

        private void SetFormValues()
        {
            lblASXCode.Text = _StockService.GetASXCode(_Dividend.Stock);
            dtpRecordDate.Value = _Dividend.ActionDate;
            dtpPaymentDate.Value = _Dividend.PaymentDate;
            txtDividendAmount.Text =  MathUtils.FormatCurrency(_Dividend.DividendAmount, false);
            txtPercentFranked.Text = (_Dividend.PercentFranked * 100).ToString("#0");
            txtCompanyTaxRate.Text = (_Dividend.CompanyTaxRate * 100).ToString("#0");
            txtDescription.Text = _Dividend.Description;
            txtDRPPrice.Text = MathUtils.FormatCurrency(_Dividend.DRPPrice, false);
        }

        public ICorporateAction CreateCorporateAction(Stock stock)
        {
            _Stock = stock;
            _Mode = Mode.Create;

            lblASXCode.Text = stock.ASXCode;
            txtCompanyTaxRate.Text = "30";

            if (ShowDialog() == DialogResult.OK)
            {
                return _Dividend;
            }
            else
                return null;
        }

        public bool EditCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Edit;
            _Dividend = corporateAction as Dividend;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _Dividend.ActionDate = dtpRecordDate.Value;
                _Dividend.PaymentDate = dtpPaymentDate.Value;
                _Dividend.DividendAmount = MathUtils.ParseDecimal(txtDividendAmount.Text);
                _Dividend.PercentFranked = MathUtils.ParseDecimal(txtPercentFranked.Text) / 100;
                _Dividend.CompanyTaxRate = MathUtils.ParseDecimal(txtCompanyTaxRate.Text, 3.0m) / 100;
                _Dividend.DRPPrice = MathUtils.ParseDecimal(txtDRPPrice.Text);
                _Dividend.Description = txtDescription.Text;

                _CorporateActionService.UpdateCorporateAction(_Dividend);

                return true;
            }
            else
                return false;
        }

        public void ViewCorporateAction(ICorporateAction corporateAction)
        {
            _Mode = Mode.View;
            _Dividend = corporateAction as Dividend;
            SetFormValues();
            ShowDialog();
        }

        public Boolean DeleteCorporateAction(ICorporateAction corporateAction)
        {
            _Stock = _StockService.GetStock(corporateAction.Stock);
            _Mode = Mode.Delete;
            _Dividend = corporateAction as Dividend;
            SetFormValues();
            if (ShowDialog() == DialogResult.OK)
            {
                _CorporateActionService.DeleteCorporateAction(_Dividend);
                return true;
            }
            return
                false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_Mode == Mode.Create)
            {
                _Dividend = new Dividend(_Stock.Id, dtpRecordDate.Value, 
                                               dtpPaymentDate.Value,
                                               MathUtils.ParseDecimal(txtDividendAmount.Text), 
                                               MathUtils.ParseDecimal(txtPercentFranked.Text) / 100, 
                                               MathUtils.ParseDecimal(txtCompanyTaxRate.Text, 3.0m) / 100, 
                                               MathUtils.ParseDecimal(txtDRPPrice.Text), 
                                               txtDescription.Text);
                _CorporateActionService.AddCorporateAction(_Dividend);
            }
        }

    }
}
