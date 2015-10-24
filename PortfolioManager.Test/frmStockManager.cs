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

    public delegate void CorporateActionAdded(ICorporateAction corporateAction);


    public partial class frmStockManager : Form
    {
        private CorporateActionFormFactory _CorporateActionFormFactory;
        private StockManager _StockManager;

        //events
        public CorporateActionAdded CorparateActionAdded;

        private frmStockManager()
        {
            InitializeComponent();
        }

        public frmStockManager(StockManager stockManager) : this()
        {
            _StockManager = stockManager;
            _CorporateActionFormFactory = new CorporateActionFormFactory(_StockManager);
        }

        private void btnAddStock_Click(object sender, EventArgs e)
        {
            if (frmStock.AddStock(_StockManager) == DialogResult.OK)
                LoadStockList();               
        }

        private void frmStockManager_Shown(object sender, EventArgs e)
        {
            LoadStockList();
        }

        private void LoadStockList()
        {
            lsvStocks.Items.Clear();
            foreach (Stock stock in _StockManager.GetStocks())
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
            ICorporateAction corporateAction = form.CreateCorporateAction(stock);

            if (corporateAction != null)
            {
                DisplayCorporateActions(stock);

                if (CorparateActionAdded != null)
                    CorparateActionAdded(corporateAction);
            }
                       
        }

        private void DisplayCorporateActions(Stock stock)
        {
            lsvCorporateActions.Items.Clear();

            IEnumerable<ICorporateAction> corporateActions = stock.GetCorporateActions();
            foreach (ICorporateAction corporateAction in corporateActions)
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

                    _StockManager.Delete(stock);
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
                ICorporateAction corporateAction = (ICorporateAction)lsvCorporateActions.FocusedItem.Tag;

                ICorporateActionForm form = _CorporateActionFormFactory.CreateCorporateActionForm(corporateAction.Type);
                if (form.EditCorporateAction(corporateAction))
                {
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
                    ICorporateAction corporateAction = (ICorporateAction)item.Tag;

                    stock.DeleteCorporateAction(corporateAction);
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

                IEnumerable<DownloadedDividendRecord> dividends = downloadService.DownloadDividendHistory(stock.ASXCode);
                foreach (DownloadedDividendRecord dividend in dividends)
                {
                    if (! DividendExists(dividend.RecordDate))
                        stock.AddDividend(dividend.RecordDate, dividend.PaymentDate, dividend.Amount, dividend.PercentFranked, 0.30m, "");
                }

                DisplayCorporateActions(stock);
            }
        }

        private Boolean DividendExists(DateTime recordDate)
        {
            foreach (ListViewItem item in lsvCorporateActions.Items)
            {
                ICorporateAction corporateAction = (ICorporateAction)item.Tag;

                if ((corporateAction.Type == CorporateActionType.Dividend) && (corporateAction.ActionDate == recordDate))
                {
                    return true;
                }
            }

            return false;
        }

        private void btnImportPrices_Click(object sender, EventArgs e)
        {
            Stock stock;

            stock = _StockManager.GetStock("AGI", DateTime.Today);
            stock.AddPrice(DateTime.Today, 2.980m);
            stock = _StockManager.GetStock("ARG", DateTime.Today);
            stock.AddPrice(DateTime.Today, 7.70m);
            stock = _StockManager.GetStock("BHP", DateTime.Today);
            stock.AddPrice(DateTime.Today, 24.590m);
            stock = _StockManager.GetStock("COH", DateTime.Today);
            stock.AddPrice(DateTime.Today, 84.200m);
            stock = _StockManager.GetStock("CSL", DateTime.Today);
            stock.AddPrice(DateTime.Today, 90.390m);
            stock = _StockManager.GetStock("WAM", DateTime.Today);
            stock.AddPrice(DateTime.Today, 1.960m);

        }

    }
}
