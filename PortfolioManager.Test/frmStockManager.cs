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
using PortfolioManager.Model.Data;
using PortfolioManager.Model.Utils;
using StockManager.Service;

namespace PortfolioManager.Test
{

    public delegate void CorporateActionAdded(CorporateAction corporateAction);


    public partial class frmStockManager : Form
    {
        private CorporateActionFormFactory _CorporateActionFormFactory;
        private StockServiceRepository _StockServiceRepository;

        //events
        public CorporateActionAdded CorporateActionAdded;

        private frmStockManager()
        {
            InitializeComponent();
        }

        public frmStockManager(IStockDatabase stockDatabase) : this()
        {
            _StockServiceRepository = new StockServiceRepository(stockDatabase);
            _CorporateActionFormFactory = new CorporateActionFormFactory(_StockServiceRepository.StockService);
        }

        private void btnAddStock_Click(object sender, EventArgs e)
        {
            if (frmStock.AddStock(_StockServiceRepository.StockService) == DialogResult.OK)
                LoadStockList();               
        }

        private void frmStockManager_Shown(object sender, EventArgs e)
        {
            LoadStockList();
        }

        private void LoadStockList()
        {
            lsvStocks.Items.Clear();
            foreach (Stock stock in _StockServiceRepository.StockService.GetStocks())
            {
                ListViewItem item = lsvStocks.Items.Add(stock.ASXCode);
                item.Tag = stock;
                if (stock.ToDate < DateTime.Today)
                {
                    item.SubItems.Add(String.Format("{0} (delisted {1:d})", stock.Name, stock.ToDate));
                    item.ForeColor = Color.LightGray;
                }
                else
                {
                    item.SubItems.Add(stock.Name);
                }              
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStockList(); 
        }

        private void btnAddDividend_Click(object sender, EventArgs e)
        {
            if (lsvStocks.FocusedItem != null)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;
                AddCorporateAction(stock, CorporateActionType.Dividend);
            }

        }

        private void AddCorporateAction(Stock stock, CorporateActionType type)
        {
            ICorporateActionForm form = _CorporateActionFormFactory.CreateCorporateActionForm(type);
            var corporateAction = form.CreateCorporateAction(stock);

            if (corporateAction != null)
            {
                _StockServiceRepository.CorporateActionService.AddCorporateAction(corporateAction); 

                DisplayCorporateActions(stock);

                if (CorporateActionAdded != null)
                    CorporateActionAdded(corporateAction);
            }
                       
        }

        private void DisplayCorporateActions(Stock stock)
        {
            lsvCorporateActions.Items.Clear();

            IEnumerable<CorporateAction> corporateActions = _StockServiceRepository.CorporateActionService.GetCorporateActions(stock);
            foreach (var corporateAction in corporateActions)
            {
                ListViewItem item = lsvCorporateActions.Items.Add(corporateAction.ActionDate.ToShortDateString());
                item.SubItems.Add(corporateAction.Description);
                item.Tag = corporateAction;
            }
        }

        private void lsvStocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvStocks.FocusedItem != null)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;
                DisplayCorporateActions(stock);
            }
        }

        private void lsvCorporateActions_ItemActivate(object sender, EventArgs e)
        {
            mnuEditCorporateAction_Click(sender, e);
        }

        private void mnuDeleteStock_Click(object sender, EventArgs e)
        {
            string message;

            if (lsvStocks.SelectedItems.Count > 1)
                message = String.Format("Are you sure you want to delete these {0} stocks?", lsvStocks.SelectedItems.Count);
            else
                message = String.Format("Are you sure you want to delete {0}?", lsvStocks.SelectedItems[0].Text);

            if (MessageBox.Show(message, "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (ListViewItem item in lsvStocks.SelectedItems)
                {
                    Stock stock = (Stock)item.Tag;

                    _StockServiceRepository.StockService.Delete(stock);
                    lsvStocks.Items.Remove(item);
                }
            }
        }

        private void mnuRenameStock_Click(object sender, EventArgs e)
        {
            // TODO: Priority Low, rename stock
        }

        private void mnuChangeASXCode_Click(object sender, EventArgs e)
        {
            // TODO: Priority Low, change asx code
        }

        private void mnuDelistStock_Click(object sender, EventArgs e)
        {
            // TODO: Priority Low, delist stock
        }

        private void mnuEditCorporateAction_Click(object sender, EventArgs e)
        {
            if (lsvCorporateActions.FocusedItem != null)
            {
                var corporateAction = (CorporateAction)lsvCorporateActions.FocusedItem.Tag;

                ICorporateActionForm form = _CorporateActionFormFactory.CreateCorporateActionForm(corporateAction.Type);
                if (form.EditCorporateAction(corporateAction))
                {
                    _StockServiceRepository.CorporateActionService.UpdateCorporateAction(corporateAction);
                    lsvCorporateActions.FocusedItem.Text = corporateAction.ActionDate.ToShortDateString();
                    lsvCorporateActions.FocusedItem.SubItems[1].Text = corporateAction.Description;
                }
            }
        }

        private void mnuDeleteCorporateAction_Click(object sender, EventArgs e)
        {
            string message;

            if (lsvCorporateActions.SelectedItems.Count > 1)
                message = String.Format("Are you sure you want to delete these {0} corporate actions?", lsvCorporateActions.SelectedItems.Count);
            else
                message = String.Format("Are you sure you want to delete {0}?", lsvCorporateActions.SelectedItems[0].SubItems[1].Text);

            if (MessageBox.Show(message, "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;

                foreach (ListViewItem item in lsvCorporateActions.SelectedItems)
                {
                    var corporateAction = (CorporateAction)item.Tag;

                    _StockServiceRepository.CorporateActionService.DeleteCorporateAction(corporateAction);
                }

                DisplayCorporateActions(stock);
            }
        }

        private void btnAddTransformation_Click(object sender, EventArgs e)
        {
            if (lsvStocks.FocusedItem != null)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;
                AddCorporateAction(stock, CorporateActionType.Transformation);
            }
        }

        private void btnAddCapitalReturn_Click(object sender, EventArgs e)
        {
            if (lsvStocks.FocusedItem != null)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;
                AddCorporateAction(stock, CorporateActionType.CapitalReturn);
            }
        }

        private void btnDownloadDividends_Click(object sender, EventArgs e)
        {
            if (lsvStocks.FocusedItem != null)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;

                DownloadService downloadService = new DownloadService();

                IEnumerable<DownloadedDividendRecord> dividendRecords = downloadService.DownloadDividendHistory(stock.ASXCode);
                foreach (DownloadedDividendRecord dividendRecord in dividendRecords)
                {
                    if (! DividendExists(dividendRecord.RecordDate))
                    {
                       var dividend = new Dividend(stock.Id, dividendRecord.RecordDate, dividendRecord.PaymentDate, dividendRecord.Amount, dividendRecord.PercentFranked, 0.30m, "");
                       _StockServiceRepository.CorporateActionService.AddCorporateAction(dividend);
                    }
                }

                DisplayCorporateActions(stock);
            }
        }

        private Boolean DividendExists(DateTime recordDate)
        {
            foreach (ListViewItem item in lsvCorporateActions.Items)
            {
                var corporateAction = (CorporateAction)item.Tag;

                if ((corporateAction.Type == CorporateActionType.Dividend) && (corporateAction.ActionDate == recordDate))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnImportPrices_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Multiselect = true;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in openDialog.FileNames)
                    _StockServiceRepository.ImportStockPrices(fileName);
            }
        }

        private void btnAddSplitConsolidation_Click(object sender, EventArgs e)
        {
            if (lsvStocks.FocusedItem != null)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;
                AddCorporateAction(stock, CorporateActionType.SplitConsolidation);
            }
        }

        private void btnAddCompositeAction_Click(object sender, EventArgs e)
        {
            if (lsvStocks.FocusedItem != null)
            {
                Stock stock = (Stock)lsvStocks.FocusedItem.Tag;
                AddCorporateAction(stock, CorporateActionType.Composite);
            }
        }
    }
}
