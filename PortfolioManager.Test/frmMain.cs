using System;
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
using PortfolioManager.Data.Memory.Stocks;
using PortfolioManager.Data.Memory.Portfolios;
using PortfolioManager.Data.SQLite.Stocks;

namespace PortfolioManager.Test 
{
    public enum Mode { Create, Display, Edit, Delete };

    public partial class frmMain : Form
    {
        private PortfolioManagerSettings _Settings;
        private PortfolioManager.Model.Portfolios.PortfolioManager _PortfolioManager;
        private Portfolio _MyPortfolio;

        private class ListViewTransactionComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                ITransaction transactionX = (ITransaction)((ListViewItem)x).Tag;
                ITransaction transactionY = (ITransaction)((ListViewItem)y).Tag;

                return DateTime.Compare(transactionX.TransactionDate, transactionY.TransactionDate);
            }
        }

        public frmMain()
        {
            InitializeComponent();

            lsvTransactions2.ListViewItemSorter = new ListViewTransactionComparer();

            _Settings = PortfolioManagerSettings.Load();
            if (_Settings != null)
                LoadDatabase();
            else
            {
                _Settings = new PortfolioManagerSettings();
                btnSettings_Click(null, null);
            }
        }

        private void LoadDatabase()
        {
            bool newDatabase = !System.IO.File.Exists(_Settings.StockDatabaseFile);
            IStockDatabase stockDatabase = new SQLiteStockDatabase("Data Source=" +  _Settings.StockDatabaseFile + ";Version=3;");
            IPortfolioDatabase portfolioDatabase = new MemoryPortfolioDatabase();

            _PortfolioManager = new PortfolioManager.Model.Portfolios.PortfolioManager(stockDatabase, portfolioDatabase);

            if (newDatabase)
            {
                AddStocks();
                AddCorporateActions();
            }


            _MyPortfolio = _PortfolioManager.CreatePortfolio("Craig's Shares");

            /* TODO: should add this when purchasing */
            var stockSetting = new StockSetting("ARG")
            {
                DRPActive = true
            };
            _MyPortfolio.StockSetting.Add("ARG", stockSetting);

            DisplayTransactions();
        }

        private void AddStocks()
        {

            _PortfolioManager.StockManager.AddStock("ADZ", "Adsteam Marine");
            _PortfolioManager.StockManager.AddStock("ARG", "Argo Investments");
            _PortfolioManager.StockManager.AddStock("FGL", "Fosters");
            _PortfolioManager.StockManager.AddStock("AMP", "AMP Limited");
            _PortfolioManager.StockManager.AddStock("HHG", "Henderson Group PLC");
            _PortfolioManager.StockManager.AddStock("NAB", "National Australia Bank");
            _PortfolioManager.StockManager.AddStock("TLS", "Telstra");
            _PortfolioManager.StockManager.AddStock("AGI", "Ainsworth Game Tech");
            _PortfolioManager.StockManager.AddStock("BCA", "Baycorp Advantage Limited");
            _PortfolioManager.StockManager.AddStock("TWE", "Treasury Wine Estate", new DateTime(2011, 05, 20));
            _PortfolioManager.StockManager.AddStock("AGIR", "Ainsworth Game Tech Rights 2005", new DateTime(2005, 08, 29));

            var WDC = _PortfolioManager.StockManager.AddStock("WDC", "Westfield Group", StockType.StapledSecurity);
            var WSF = _PortfolioManager.StockManager.AddStock("WSF", "Westfield Holdings Limited", StockType.Ordinary, WDC);
            var WFT = _PortfolioManager.StockManager.AddStock("WFT", "Westfield Trust", StockType.Trust, WDC);
            var WFA = _PortfolioManager.StockManager.AddStock("WFA", "Westfield America Trust", StockType.Trust, WDC);

            /* Assume nta prior to merger is same as december */
            WSF.AddRelativeNTA(new DateTime(2004, 06, 30), 0.0924M);
            WFT.AddRelativeNTA(new DateTime(2004, 06, 30), 0.4986M);
            WFA.AddRelativeNTA(new DateTime(2004, 06, 30), 0.4090M);
            WSF.AddRelativeNTA(new DateTime(2004, 12, 31), 0.0924M);
            WFT.AddRelativeNTA(new DateTime(2004, 12, 31), 0.4986M);
            WFA.AddRelativeNTA(new DateTime(2004, 12, 31), 0.4090M);
            WSF.AddRelativeNTA(new DateTime(2005, 06, 30), 0.0711M);
            WFT.AddRelativeNTA(new DateTime(2005, 06, 30), 0.5195M);
            WFA.AddRelativeNTA(new DateTime(2005, 06, 30), 0.4094M);
            WSF.AddRelativeNTA(new DateTime(2005, 12, 31), 0.0805M);
            WFT.AddRelativeNTA(new DateTime(2005, 12, 31), 0.5166M);
            WFA.AddRelativeNTA(new DateTime(2005, 12, 31), 0.4029M);
            WSF.AddRelativeNTA(new DateTime(2006, 06, 30), 0.0802M);
            WFT.AddRelativeNTA(new DateTime(2006, 06, 30), 0.5490M);
            WFA.AddRelativeNTA(new DateTime(2006, 06, 30), 0.3708M);
            WSF.AddRelativeNTA(new DateTime(2006, 12, 31), 0.0738M);
            WFT.AddRelativeNTA(new DateTime(2006, 12, 31), 0.5843M);
            WFA.AddRelativeNTA(new DateTime(2006, 12, 31), 0.3419M);
            WSF.AddRelativeNTA(new DateTime(2007, 06, 30), 0.0754M);
            WFT.AddRelativeNTA(new DateTime(2007, 06, 30), 0.5922M);
            WFA.AddRelativeNTA(new DateTime(2007, 06, 30), 0.3324M);
            WSF.AddRelativeNTA(new DateTime(2007, 12, 31), 0.0807M);
            WFT.AddRelativeNTA(new DateTime(2007, 12, 31), 0.6246M);
            WFA.AddRelativeNTA(new DateTime(2007, 12, 31), 0.2947M);
            WSF.AddRelativeNTA(new DateTime(2008, 06, 30), 0.0683M);
            WFT.AddRelativeNTA(new DateTime(2008, 06, 30), 0.6529M);
            WFA.AddRelativeNTA(new DateTime(2008, 06, 30), 0.2788M);
            WSF.AddRelativeNTA(new DateTime(2008, 12, 31), 0.0570M);
            WFT.AddRelativeNTA(new DateTime(2008, 12, 31), 0.6572M);
            WFA.AddRelativeNTA(new DateTime(2008, 12, 31), 0.2858M);
            WSF.AddRelativeNTA(new DateTime(2009, 06, 30), 0.0463M);
            WFT.AddRelativeNTA(new DateTime(2009, 06, 30), 0.7132M);
            WFA.AddRelativeNTA(new DateTime(2009, 06, 30), 0.2405M);
            WSF.AddRelativeNTA(new DateTime(2009, 12, 31), 0.0401M);
            WFT.AddRelativeNTA(new DateTime(2009, 12, 31), 0.7459M);
            WFA.AddRelativeNTA(new DateTime(2009, 12, 31), 0.2140M);
            WSF.AddRelativeNTA(new DateTime(2010, 06, 30), 0.0315M);
            WFT.AddRelativeNTA(new DateTime(2010, 06, 30), 0.7454M);
            WFA.AddRelativeNTA(new DateTime(2010, 06, 30), 0.2231M);
            WSF.AddRelativeNTA(new DateTime(2010, 12, 31), 0.0404M);
            WFT.AddRelativeNTA(new DateTime(2010, 12, 31), 0.6727M);
            WFA.AddRelativeNTA(new DateTime(2010, 12, 31), 0.2869M);
            WSF.AddRelativeNTA(new DateTime(2011, 06, 30), 0.0396M);
            WFT.AddRelativeNTA(new DateTime(2011, 06, 30), 0.6832M);
            WFA.AddRelativeNTA(new DateTime(2011, 06, 30), 0.2772M);
            WSF.AddRelativeNTA(new DateTime(2011, 12, 31), 0.0358M);
            WFT.AddRelativeNTA(new DateTime(2011, 12, 31), 0.6850M);
            WFA.AddRelativeNTA(new DateTime(2011, 12, 31), 0.2792M);
            WSF.AddRelativeNTA(new DateTime(2012, 06, 30), 0.0471M);
            WFT.AddRelativeNTA(new DateTime(2012, 06, 30), 0.7541M);
            WFA.AddRelativeNTA(new DateTime(2012, 06, 30), 0.1988M);
            WSF.AddRelativeNTA(new DateTime(2012, 12, 31), 0.0536M);
            WFT.AddRelativeNTA(new DateTime(2012, 12, 31), 0.7551M);
            WFA.AddRelativeNTA(new DateTime(2012, 12, 31), 0.1913M);
            WSF.AddRelativeNTA(new DateTime(2013, 06, 30), 0.0672M);
            WFT.AddRelativeNTA(new DateTime(2013, 06, 30), 0.7382M);
            WFA.AddRelativeNTA(new DateTime(2013, 06, 30), 0.1946M);
            WSF.AddRelativeNTA(new DateTime(2013, 12, 31), 0.1070M);
            WFT.AddRelativeNTA(new DateTime(2013, 12, 31), 0.7188M);
            WFA.AddRelativeNTA(new DateTime(2013, 12, 31), 0.1742M);
        }

        private void AddCorporateActions()
        {

            /* Fosters */
            var FGL = _PortfolioManager.StockManager.GetStock("FGL");
            FGL.AddDividend(new DateTime(2004, 09, 01), new DateTime(2004, 10, 01), 0.105M, 1.00M, 0.30M, "");
            FGL.AddDividend(new DateTime(2005, 09, 09), new DateTime(2005, 10, 03), 0.1075M, 1.00M, 0.30M, "");
            FGL.AddDividend(new DateTime(2006, 03, 08), new DateTime(2006, 04, 03), 0.0975M, 1.00M, 0.30M, "");

            var TWEDemerger = FGL.AddTransformation(new DateTime(2011, 5, 16), new DateTime(2011, 05, 20), 0.00M, "TWE Demerger");
            var TWE = _PortfolioManager.StockManager.GetStock("TWE");
            TWEDemerger.AddResultStock(TWE.Id, 3, 1, 0.2004M);

            /* Baycorp Advantage */
            var BCA = _PortfolioManager.StockManager.GetStock("BCA");
            BCA.AddDividend(new DateTime(2005, 02, 25), new DateTime(2005, 03, 23), 0.06M, 1.00M, 0.30M, "");
            BCA.AddDividend(new DateTime(2005, 09, 05), new DateTime(2005, 09, 26), 0.08M, 1.00M, 0.30M, "");
            BCA.AddCapitalReturn(new DateTime(2005, 11, 17), new DateTime(2005, 11, 17), 0.50M, "");
            BCA.AddDividend(new DateTime(2006, 02, 27), new DateTime(2006, 03, 23), 0.06M, 1.00M, 0.30M, "");
            BCA.AddDividend(new DateTime(2006, 09, 04), new DateTime(2006, 09, 26), 0.08M, 1.00M, 0.30M, "");
            BCA.AddDividend(new DateTime(2006, 11, 02), new DateTime(2006, 11, 17), 0.35M, 1.00M, 0.30M, "");
            BCA.ChangeASXCode(new DateTime(2006, 11, 03), "VEA", "Veda Advantage Limited");
            var VEA = _PortfolioManager.StockManager.GetStock("VEA");
            BCA.AddDividend(new DateTime(2007, 02, 26), new DateTime(2007, 03, 22), 0.06M, 1.00M, 0.30M, "");
            BCA.AddDividend(new DateTime(2007, 06, 26), new DateTime(2007, 07, 06), 0.10M, 1.00M, 0.30M, "");

            var VEASchemeOfArrangement = VEA.AddTransformation(new DateTime(2007, 07, 09), new DateTime(2007, 07, 09), 3.51M, "Scheme of Arrangement");

            /* Argo */
            var ARG = _PortfolioManager.StockManager.GetStock("ARG");
            ARG.AddDividend(new DateTime(2006, 02, 24), new DateTime(2006, 03, 10), 0.11M, 1.00M, 0.30M, 6.71M, "");
            /* DRP amount: $6.71, received 5 shares (300 shares at the time ) */

            /* National Australia Bank */
            var NAB = _PortfolioManager.StockManager.GetStock("NAB");
            NAB.AddDividend(new DateTime(2005, 11, 25), new DateTime(2005, 12, 19), 0.83M, 0.8M, 0.30M, "");
            
        }

        private void AddTransaction(ITransaction transaction)
        {
            ListViewItem item;

            item = lsvTransactions2.Items.Add(transaction.TransactionDate.ToShortDateString());
            item.SubItems.Add(transaction.ASXCode);
            item.SubItems.Add(transaction.Description);
            item.Tag = transaction;
        }

        private void DisplayTransactions()
        {
            lsvTransactions2.Items.Clear();

            AddTransaction(new Aquisition(new DateTime(2002, 01, 10), "AGI", 2500, 1.14M, 16.30M, ""));

            AddTransaction(new OpeningBalance(new DateTime(2005, 09, 30), "AGIR", 804, 0.00M, "Renounceable rights issue"));

            AddTransaction(new Disposal(new DateTime(2005, 10, 14), "AGIR", 804, 0.06M, 19.95M, CGTCalculationMethod.MinimizeGain, ""));

            AddTransaction(new Aquisition(new DateTime(2002, 02, 14), "BCA", 750, 5.22M, 16.30M, ""));
            AddTransaction(new Aquisition(new DateTime(2003, 04, 28), "BCA", 1250, 1.47M, 16.30M, ""));

            AddTransaction(new OpeningBalance(new DateTime(2004, 07, 01), "ADZ", 1819, 1375.00M, ""));
            AddTransaction(new Disposal(new DateTime(2004, 11, 10), "ADZ", 1819, 1.65M, 19.95M, CGTCalculationMethod.MinimizeGain, ""));

            AddTransaction(new OpeningBalance(new DateTime(2004, 07, 01), "AMP", 1125, 4633.75M, ""));

            AddTransaction(new OpeningBalance(new DateTime(2004, 07, 01), "HHG", 1125, 1330.70M, "Demerger of HHG from AMP"));

            AddTransaction(new OpeningBalance(new DateTime(2004, 07, 01), "FGL", 1125, 4633.75M, ""));

            AddTransaction(new OpeningBalance(new DateTime(2004, 07, 01), "NAB", 150, 4366.30M, ""));

            AddTransaction(new OpeningBalance(new DateTime(2004, 07, 01), "TLS", 1150, 7679.90M, ""));

            AddTransaction(new Aquisition(new DateTime(2004, 07, 13), "WDC", 350, 15.32M, 19.95M, ""));

            AddTransaction(new Aquisition(new DateTime(2006, 01, 13), "ARG", 300, 6.52M, 19.95M, ""));       
        }

        private void DisplayCorporateActions()
        {

            var corporateActions = _MyPortfolio.GetUnappliedCorparateActions();

            lsvCorporateActions.Items.Clear();
            foreach (ICorporateAction corporateAction in corporateActions)
            {
                ListViewItem item = lsvCorporateActions.Items.Add(corporateAction.ActionDate.ToShortDateString());
                item.SubItems.Add(_PortfolioManager.StockManager.GetASXCode(corporateAction.Stock));
                item.SubItems.Add(corporateAction.Description);
                item.Tag = corporateAction;
            }         
        }

        private void DisplayPortfolio()
        {
            /* Current Holdings */
            lsvPortfolio.Items.Clear();
            var holdings = _MyPortfolio.GetHoldings(DateTime.Now);
            foreach (ShareHolding holding in holdings)
            {          
                var item = lsvPortfolio.Items.Add(holding.Stock.ASXCode);
                item.Tag = holding.Stock;
                item.SubItems.Add(holding.Units.ToString("n0"));
                item.SubItems.Add(holding.AverageUnitPrice.ToString("c"));
                item.SubItems.Add(holding.Cost.ToString("c"));
                item.SubItems.Add(holding.UnitValue.ToString("c"));
                item.SubItems.Add(holding.MarketValue.ToString("c"));
            }

            /* Parcels */
            lsvParcels.Items.Clear();
            var parcels = _MyPortfolio.GetParcels(DateTime.Now);
            foreach (ShareParcel parcel in parcels)
            {
                var item = lsvParcels.Items.Add(_PortfolioManager.StockManager.GetASXCode(parcel.Stock));
                item.SubItems.Add(parcel.Units.ToString("n0"));
                item.SubItems.Add(parcel.UnitPrice.ToString("c"));
                item.SubItems.Add(parcel.CostBase.ToString("c"));
            }

            /* CGT */
            lsvCGT.Items.Clear();
            var cgtEvents = _MyPortfolio.GetCGTEvents(new DateTime(0001, 01, 01), new DateTime(9999, 12, 31));
            foreach (CGTEvent cgtEvent in cgtEvents)
            {
                var item = lsvCGT.Items.Add(cgtEvent.EventDate.ToShortDateString());
                item.SubItems.Add(_PortfolioManager.StockManager.GetASXCode(cgtEvent.Stock, cgtEvent.EventDate));
                item.SubItems.Add(cgtEvent.CostBase.ToString("c"));
                item.SubItems.Add(cgtEvent.AmountReceived.ToString("c"));
                item.SubItems.Add(cgtEvent.CapitalGain.ToString("c"));
            }

            /* Income */
            lsvIncome.Items.Clear();
            var allIncome = _MyPortfolio.GetIncomeReceived(new DateTime(0001, 01, 01), new DateTime(9999, 12, 31));
            foreach (IncomeReceived income in allIncome)
            {
                var item = lsvIncome.Items.Add(income.TransactionDate.ToShortDateString());
                item.SubItems.Add(income.ASXCode);
                item.SubItems.Add(income.CashIncome.ToString("c"));
                item.SubItems.Add(income.FrankingCredits.ToString("c"));
            }

            /* Transactions */
            lsvTransactions.Items.Clear();
            var allTransactions = _MyPortfolio.GetTransactions(new DateTime(0001, 01, 01), new DateTime(9999, 12, 31));
            foreach (ITransaction transaction in allTransactions)
            {
                var item = lsvTransactions.Items.Add(transaction.TransactionDate.ToShortDateString());
                item.SubItems.Add(transaction.ASXCode);
                item.SubItems.Add(transaction.Description);
            }
        }

        private void lsvTransactions_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            ITransaction transaction = lsvTransactions2.FocusedItem.Tag as ITransaction;
            lsvTransactions2.FocusedItem.Remove();

            _MyPortfolio.ApplyTransaction(transaction);
     
            DisplayPortfolio();
            DisplayCorporateActions();
        }

        private void lsvCorporateActions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ICorporateAction corporateAction = lsvCorporateActions.FocusedItem.Tag as ICorporateAction;
            lsvCorporateActions.FocusedItem.Remove();

            var transactions = corporateAction.CreateTransactionList(_MyPortfolio);
            _MyPortfolio.ApplyTransactions(transactions);

            DisplayPortfolio();
        }

        private void btnAddStock_Click(object sender, EventArgs e)
        {
            frmStock.AddStock(_PortfolioManager.StockManager);
        }

        private void btnAddDividend_Click(object sender, EventArgs e)
        {
            frmDividend.AddDividend(_PortfolioManager.StockManager);

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

        private void btnAddAquisition_Click(object sender, EventArgs e)
        {
            Aquisition aquisition = frmAquisition.AddAquisition(_PortfolioManager.StockManager);
            if (aquisition != null)
            {
                _MyPortfolio.ApplyTransaction(aquisition);

                DisplayPortfolio();
                DisplayCorporateActions();
            }
            
    

        }


    }
}
