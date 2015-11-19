using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Utils;

namespace PortfolioManager.Test.TransactionControls
{
    public partial class IncomeControl : UserControl, ITransactionControl
    {
        private readonly StockService _StockService;

        public IncomeControl()
        {
            InitializeComponent();
        }

        public IncomeControl(StockService stockService)
            : this()
        {
            _StockService = stockService;

            dtpRecordDate_ValueChanged(this, null);
        }

        public ITransaction CreateTransaction()
        {
            var transaction = new IncomeReceived();
            UpdateTransaction(transaction);

            return transaction;
        }

        public void DisplayTransaction(ITransaction transaction)
        {
            IncomeReceived incomeReceived = transaction as IncomeReceived;
            
            dtpRecordDate.Value = incomeReceived.RecordDate;

            foreach (Stock s in cboASXCode.Items)
            {
                if (s.ASXCode == incomeReceived.ASXCode)
                {
                    cboASXCode.SelectedItem = s;
                    break;
                }
            };

            dtpPaymentDate.Value = incomeReceived.TransactionDate;
            txtFrankedAmount.Text = MathUtils.FormatCurrency(incomeReceived.FrankedAmount);
            txtUnfrankedAmount.Text = MathUtils.FormatCurrency(incomeReceived.UnfrankedAmount);
            txtFrankingCredits.Text = MathUtils.FormatCurrency(incomeReceived.FrankingCredits);
            txtInterest.Text = MathUtils.FormatCurrency(incomeReceived.Interest);
            txtTaxDeferred.Text = MathUtils.FormatCurrency(incomeReceived.TaxDeferred);
            txtTotalCashIncome.Text = MathUtils.FormatCurrency(incomeReceived.CashIncome);
            txtComment.Text = incomeReceived.Comment;
        }

        public void UpdateTransaction(ITransaction transaction)
        {
            IncomeReceived incomeReceived = transaction as IncomeReceived;

            Stock stock = cboASXCode.SelectedItem as Stock;
            incomeReceived.ASXCode = stock.ASXCode;
            incomeReceived.TransactionDate = dtpPaymentDate.Value;
            incomeReceived.RecordDate = dtpRecordDate.Value;
            incomeReceived.FrankedAmount = MathUtils.ParseDecimal(txtFrankedAmount.Text);
            incomeReceived.UnfrankedAmount = MathUtils.ParseDecimal(txtUnfrankedAmount.Text);
            incomeReceived.FrankingCredits = MathUtils.ParseDecimal(txtFrankingCredits.Text);
            incomeReceived.Interest = MathUtils.ParseDecimal(txtInterest.Text);
            incomeReceived.TaxDeferred = MathUtils.ParseDecimal(txtTaxDeferred.Text);
            incomeReceived.Comment = txtComment.Text;
        }

        private void dtpRecordDate_ValueChanged(object sender, EventArgs e)
        {
            var stockList = _StockService.GetAll(dtpRecordDate.Value).Where(x => x.Type != StockType.StapledSecurity).OrderBy(x => x.ASXCode);

            cboASXCode.Items.Clear();
            cboASXCode.Items.AddRange(stockList.ToArray());
        }
    }
}
