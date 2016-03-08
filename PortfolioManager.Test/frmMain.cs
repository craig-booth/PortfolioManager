﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PortfolioManager.Model.Portfolios;
using PortfolioManager.Model.Stocks;
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using PortfolioManager.Data.SQLite.Stocks;
using PortfolioManager.Data.SQLite.Portfolios;
using PortfolioManager.Service;

namespace PortfolioManager.Test 
{
    public enum Mode { Create, View, Edit, Delete };

    public partial class frmMain : Form
    {
        private PortfolioManagerSettings _Settings;
        private PortfolioServiceRepository _PortfolioServiceRepository;
        private IStockDatabase _StockDatabase;
        private Portfolio _MyPortfolio;

        private int _FinancialYear;

        private class ListViewTransactionComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                Transaction transactionX = (Transaction)((ListViewItem)x).Tag;
                Transaction transactionY = (Transaction)((ListViewItem)y).Tag;

                return DateTime.Compare(transactionX.TransactionDate, transactionY.TransactionDate);
            }
        }

        public frmMain()
        {
            InitializeComponent();

            lsvTransactions2.ListViewItemSorter = new ListViewTransactionComparer();

            _Settings = PortfolioManagerSettings.Load();
            if (_Settings != null)
            {
                // Temporary change to force database files to be in the application folder while database being updated
                _Settings.PortfolioDatabaseFile = System.IO.Path.Combine(Application.StartupPath, "Portfolio.db");
                _Settings.StockDatabaseFile = System.IO.Path.Combine(Application.StartupPath, "Stocks.db");

                LoadDatabase();
            }
            else
            {
                _Settings = new PortfolioManagerSettings();
                btnSettings_Click(null, null);
            }          
        }

        private void LoadDatabase()
        {         
            _StockDatabase = new SQLiteStockDatabase(_Settings.StockDatabaseFile);

            IPortfolioDatabase portfolioDatabase = new SQLitePortfolioDatabase(_Settings.PortfolioDatabaseFile);

            _PortfolioServiceRepository = new PortfolioServiceRepository(portfolioDatabase, _StockDatabase.StockQuery, _StockDatabase.CorporateActionQuery);         
            _MyPortfolio = _PortfolioServiceRepository.CreatePortfolio("Craig's Shares");

            /* TODO: Priority Low, should add this when purchasing */
            var stockSetting = new StockSetting("ARG")
            {
                DRPActive = true
            };
            _MyPortfolio.StockSetting.Add("ARG", stockSetting);

            cboFinancialYear.SelectedIndex = 0;
            DisplayCorporateActions();
            
        }

        private void AddTransaction(Transaction transaction)
        {
            ListViewItem item;

            item = lsvTransactions2.Items.Add(transaction.TransactionDate.ToShortDateString());
            item.SubItems.Add(transaction.ASXCode);
            item.SubItems.Add(transaction.Description);
            item.Tag = transaction;
        }

        private void DisplayCorporateActions()
        {

            var corporateActions = _MyPortfolio.CorporateActionService.GetUnappliedCorporateActions();
            lsvCorporateActions.Items.Clear();
            foreach (CorporateAction corporateAction in corporateActions)
            {
                ListViewItem item = lsvCorporateActions.Items.Add(corporateAction.ActionDate.ToShortDateString());
                item.SubItems.Add(_PortfolioServiceRepository.StockService.Get(corporateAction.Stock, corporateAction.ActionDate).ASXCode);
                item.SubItems.Add(corporateAction.Description);
                item.Tag = corporateAction;
            }         
        }

        private void DisplayPortfolio()
        {
            DateTime startDate;
            DateTime endDate;
            if (_FinancialYear > 0)
            {
                startDate = new DateTime(_FinancialYear - 1, 07, 01);
                endDate = new DateTime(_FinancialYear, 06, 30);
            }
            else
            {
                startDate = DateTimeConstants.NoStartDate;
                endDate = DateTimeConstants.NoEndDate;             
            }

            /* Current Holdings */
            decimal totalCostBase = 0.00m;
            decimal totalMarketValue = 0.00m;
            decimal totalCapitalGain = 0.00m;
            decimal totalCapitalGainPercentage = 0.00m;

            lsvPortfolio.Items.Clear();
            var holdings = _MyPortfolio.ShareHoldingService.GetHoldings(endDate).OrderBy(x => x.Stock.ASXCode);
            foreach (ShareHolding holding in holdings)
            {          
                decimal capitalGain;
                decimal capitalGainPercentage;

                capitalGain = (holding.MarketValue - holding.TotalCostBase);
                if (holding.TotalCostBase > 0.00m)
                    capitalGainPercentage  = capitalGain / holding.TotalCostBase;
                else
                    capitalGainPercentage = 0.00m;

                totalCostBase += holding.TotalCostBase;
                totalMarketValue += holding.MarketValue;
                totalCapitalGain += capitalGain;                

                var item = lsvPortfolio.Items.Add(holding.Stock.ASXCode);
                item.Tag = holding.Stock;
                item.SubItems.Add(holding.Units.ToString("n0"));
                item.SubItems.Add(MathUtils.FormatCurrency(holding.AverageUnitCost, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(holding.TotalCostBase, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(holding.UnitValue, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(holding.MarketValue, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(capitalGain, true, true));
                item.SubItems.Add(capitalGainPercentage.ToString("##0.0%"));
            }
            lblTotalCostBase.Text = MathUtils.FormatCurrency(totalCostBase, true, true);
            lblTotalMarketValue.Text = MathUtils.FormatCurrency(totalMarketValue, true, true);
            lblTotalCapitalGain.Text = MathUtils.FormatCurrency(totalCapitalGain, true, true);
            totalCapitalGainPercentage = totalCapitalGain / totalCostBase;
            lblTotalCapitalGainPercentage.Text = totalCapitalGainPercentage.ToString("##0.0%");


            /* Parcels */
            var discountDate = DateTime.Today.AddYears(-1);
            lsvParcels.Items.Clear();
            var parcels = _MyPortfolio.ParcelService.GetParcels(endDate).OrderBy(x => x.Stock).ThenBy(x => x.AquisitionDate);
            foreach (ShareParcel parcel in parcels)
            {
                var stock = _PortfolioServiceRepository.StockService.Get(parcel.Stock, endDate);
                var closingPrice = _PortfolioServiceRepository.StockPriceService.GetClosingPrice(stock, endDate);
                var marketValue = parcel.Units * closingPrice;
                var capitalGain = marketValue - parcel.CostBase;
                decimal capitalGainPercentage = 0.00m;
                if (parcel.CostBase > 0.00m)
                    capitalGainPercentage = capitalGain / parcel.CostBase;
                decimal discountedCapitalGain;
                if (parcel.AquisitionDate.CompareTo(discountDate) < 0)
                    discountedCapitalGain = capitalGain / 2;
                else
                    discountedCapitalGain = capitalGain;

                var item = lsvParcels.Items.Add(stock.ASXCode);
                item.SubItems.Add(parcel.Units.ToString("n0"));
                item.SubItems.Add(MathUtils.FormatCurrency(parcel.UnitPrice, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(parcel.CostBase, true, true));                
                item.SubItems.Add(MathUtils.FormatCurrency(marketValue, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(capitalGain, true, true));
                item.SubItems.Add(capitalGainPercentage.ToString("##0.0%"));
                item.SubItems.Add(MathUtils.FormatCurrency(discountedCapitalGain, true, true));
            }

            /* CGT */
            lsvCGT.Items.Clear();
            var cgtEvents = _MyPortfolio.CGTService.GetEvents(startDate, endDate);
            foreach (CGTEvent cgtEvent in cgtEvents)
            {
                var item = lsvCGT.Items.Add(cgtEvent.EventDate.ToShortDateString());
                item.SubItems.Add(_PortfolioServiceRepository.StockService.Get(cgtEvent.Stock, cgtEvent.EventDate).ASXCode);
                item.SubItems.Add(MathUtils.FormatCurrency(cgtEvent.CostBase, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(cgtEvent.AmountReceived, true, true));
                item.SubItems.Add(MathUtils.FormatCurrency(cgtEvent.CapitalGain, true, true));
            }

            /* Income */
            lsvIncome.Items.Clear();
            var allIncome = _MyPortfolio.IncomeService.GetIncome(startDate, endDate);
            foreach (Income income in allIncome)
            {
                var item = lsvIncome.Items.Add(income.ASXCode);
                item.SubItems.Add(MathUtils.FormatCurrency(income.CashIncome, true));
                item.SubItems.Add(MathUtils.FormatCurrency(income.FrankingCredits, true));
            }

            /* Transactions */
            lsvTransactions.Items.Clear();
            var allTransactions = _MyPortfolio.TransactionService.GetTransactions(startDate, endDate);
            foreach (Transaction transaction in allTransactions)
            {
                var item = lsvTransactions.Items.Add(transaction.TransactionDate.ToShortDateString());
                item.SubItems.Add(transaction.ASXCode);
                item.SubItems.Add(transaction.Description);
                item.Tag = transaction;
            }

            /* Cash Account */
            lsvCashAccount.Items.Clear();
                
        }

        private void lsvTransactions_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            var transaction = lsvTransactions2.FocusedItem.Tag as Transaction;
            lsvTransactions2.FocusedItem.Remove();

            _MyPortfolio.TransactionService.ProcessTransaction(transaction);
     
            DisplayPortfolio();
            DisplayCorporateActions();
        }

        private void lsvCorporateActions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var corporateAction = lsvCorporateActions.FocusedItem.Tag as CorporateAction;
            
            var transactions = _MyPortfolio.CorporateActionService.CreateTransactionList(corporateAction);

            var form = new frmMultipleTransactions(_PortfolioServiceRepository.StockService);
            if (form.EditTransactions(transactions))
            {
                _MyPortfolio.TransactionService.ProcessTransactions(transactions);
                lsvCorporateActions.FocusedItem.Remove();

                DisplayPortfolio();
            }
        }

        private void btnStockManager_Click(object sender, EventArgs e)
        {
            var stockManagerForm = new frmStockManager(_StockDatabase);
            stockManagerForm.CorporateActionAdded += CorporateActionAdded;
            stockManagerForm.ShowDialog();
        }


        private void CorporateActionAdded(CorporateAction corporateAction)
        {
            DisplayCorporateActions();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmSettings settingsForm = new frmSettings(_Settings);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                LoadDatabase();
            }
        }

        private void AddTransaction(TransactionType type)
        {
            var form = new frmTransaction(_PortfolioServiceRepository.StockService, _MyPortfolio.AttachmentService);

            var transaction = form.CreateTransaction(type);
            if (transaction != null)
            {
                _MyPortfolio.TransactionService.ProcessTransaction(transaction);

                DisplayPortfolio();
                DisplayCorporateActions();
            }    
        }

        private void cboFinancialYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboFinancialYear.Text == "--All--")
                _FinancialYear = 0;
            else if (cboFinancialYear.Text == "--Current--")
            {
                if (DateTime.Today.Month <= 5)
                    _FinancialYear = DateTime.Today.Year;
                else
                    _FinancialYear = DateTime.Today.Year + 1;
            }
            else
                _FinancialYear = int.Parse(cboFinancialYear.Text.Substring(0, 4)) + 1;

            DisplayPortfolio();
        }

        private void btnAddAquisition_Click(object sender, EventArgs e)
        {
            AddTransaction(TransactionType.Aquisition);
        }

        private void btnAddDisposal_Click(object sender, EventArgs e)
        {
            AddTransaction(TransactionType.Disposal);
        }

        private void btnAddOpeningBalance_Click(object sender, EventArgs e)
        {
            AddTransaction(TransactionType.OpeningBalance);
        }

        private void btnAddIncomeReceived_Click(object sender, EventArgs e)
        {
            AddTransaction(TransactionType.Income);
        }

        private void btnAddReturnOfCapital_Click(object sender, EventArgs e)
        {
            AddTransaction(TransactionType.ReturnOfCapital);
        }

        private void btnAddCostbaseAdjustment_Click(object sender, EventArgs e)
        {
            AddTransaction(TransactionType.CostBaseAdjustment);
        }

        private void ctxTransaction_Opening(object sender, CancelEventArgs e)
        {
            if (lsvTransactions.FocusedItem == null)
                e.Cancel = true;
        }

        private void mnuEditTransaction_Click(object sender, EventArgs e)
        {          
            var form = new frmTransaction(_PortfolioServiceRepository.StockService, _MyPortfolio.AttachmentService);

            var transaction = (Transaction)lsvTransactions.FocusedItem.Tag;
            if (form.EditTransaction(transaction))
            {
                _MyPortfolio.TransactionService.UpdateTransaction(transaction);

                DisplayPortfolio();
                DisplayCorporateActions();
            }  
        }

        private void mnuDeleteTransaction_Click(object sender, EventArgs e)
        {
            var form = new frmTransaction(_PortfolioServiceRepository.StockService, _MyPortfolio.AttachmentService);

            var transaction = (Transaction)lsvTransactions.FocusedItem.Tag;
            if (form.DeleteTransaction(transaction))
            {
                _MyPortfolio.TransactionService.DeleteTransaction(transaction);

                DisplayPortfolio();
                DisplayCorporateActions();
            }  
        }

        private void lsvTransactions_ItemActivate(object sender, EventArgs e)
        {
            mnuEditTransaction_Click(sender, e);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DisplayPortfolio();
            DisplayCorporateActions();
        }

        private void unitCountAdjustmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTransaction(TransactionType.UnitCountAdjustment);
        }
    }
}
