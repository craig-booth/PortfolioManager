namespace PortfolioManager.Test
{
    partial class frmStockManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStockManager));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnAddStock = new System.Windows.Forms.ToolStripButton();
            this.btnAddCorporateAction = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddCapitalReturn = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddDividend = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddTransformation = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddSplitConsolidation = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddCompositeAction = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDownloadDividends = new System.Windows.Forms.ToolStripButton();
            this.btnImportPrices = new System.Windows.Forms.ToolStripButton();
            this.lsvStocks = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxStocks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuDeleteStock = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRenameStock = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChangeASXCode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelistStock = new System.Windows.Forms.ToolStripMenuItem();
            this.lsvCorporateActions = new System.Windows.Forms.ListView();
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxCorporateActions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuEditCorporateAction = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteCorporateAction = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.ctxStocks.SuspendLayout();
            this.ctxCorporateActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh,
            this.btnAddStock,
            this.btnAddCorporateAction,
            this.btnDownloadDividends,
            this.btnImportPrices});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1012, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnAddStock
            // 
            this.btnAddStock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddStock.Image = ((System.Drawing.Image)(resources.GetObject("btnAddStock.Image")));
            this.btnAddStock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddStock.Name = "btnAddStock";
            this.btnAddStock.Size = new System.Drawing.Size(65, 22);
            this.btnAddStock.Text = "Add Stock";
            this.btnAddStock.Click += new System.EventHandler(this.btnAddStock_Click);
            // 
            // btnAddCorporateAction
            // 
            this.btnAddCorporateAction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddCorporateAction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddCapitalReturn,
            this.btnAddDividend,
            this.btnAddTransformation,
            this.btnAddSplitConsolidation,
            this.btnAddCompositeAction});
            this.btnAddCorporateAction.Image = ((System.Drawing.Image)(resources.GetObject("btnAddCorporateAction.Image")));
            this.btnAddCorporateAction.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddCorporateAction.Name = "btnAddCorporateAction";
            this.btnAddCorporateAction.Size = new System.Drawing.Size(139, 22);
            this.btnAddCorporateAction.Text = "Add Corporate  Action";
            // 
            // btnAddCapitalReturn
            // 
            this.btnAddCapitalReturn.Name = "btnAddCapitalReturn";
            this.btnAddCapitalReturn.Size = new System.Drawing.Size(170, 22);
            this.btnAddCapitalReturn.Text = "Capital Return";
            this.btnAddCapitalReturn.Click += new System.EventHandler(this.btnAddCapitalReturn_Click);
            // 
            // btnAddDividend
            // 
            this.btnAddDividend.Name = "btnAddDividend";
            this.btnAddDividend.Size = new System.Drawing.Size(170, 22);
            this.btnAddDividend.Text = "Dividend";
            this.btnAddDividend.Click += new System.EventHandler(this.btnAddDividend_Click);
            // 
            // btnAddTransformation
            // 
            this.btnAddTransformation.Name = "btnAddTransformation";
            this.btnAddTransformation.Size = new System.Drawing.Size(170, 22);
            this.btnAddTransformation.Text = "Transformation";
            this.btnAddTransformation.Click += new System.EventHandler(this.btnAddTransformation_Click);
            // 
            // btnAddSplitConsolidation
            // 
            this.btnAddSplitConsolidation.Name = "btnAddSplitConsolidation";
            this.btnAddSplitConsolidation.Size = new System.Drawing.Size(170, 22);
            this.btnAddSplitConsolidation.Text = "Split/Cosolidation";
            this.btnAddSplitConsolidation.Click += new System.EventHandler(this.btnAddSplitConsolidation_Click);
            // 
            // btnAddCompositeAction
            // 
            this.btnAddCompositeAction.Name = "btnAddCompositeAction";
            this.btnAddCompositeAction.Size = new System.Drawing.Size(170, 22);
            this.btnAddCompositeAction.Text = "Composite Action";
            this.btnAddCompositeAction.Click += new System.EventHandler(this.btnAddCompositeAction_Click);
            // 
            // btnDownloadDividends
            // 
            this.btnDownloadDividends.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDownloadDividends.Image = ((System.Drawing.Image)(resources.GetObject("btnDownloadDividends.Image")));
            this.btnDownloadDividends.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDownloadDividends.Name = "btnDownloadDividends";
            this.btnDownloadDividends.Size = new System.Drawing.Size(120, 22);
            this.btnDownloadDividends.Text = "Download Dividends";
            this.btnDownloadDividends.Click += new System.EventHandler(this.btnDownloadDividends_Click);
            // 
            // btnImportPrices
            // 
            this.btnImportPrices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnImportPrices.Image = ((System.Drawing.Image)(resources.GetObject("btnImportPrices.Image")));
            this.btnImportPrices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnImportPrices.Name = "btnImportPrices";
            this.btnImportPrices.Size = new System.Drawing.Size(81, 22);
            this.btnImportPrices.Text = "Import Prices";
            this.btnImportPrices.Click += new System.EventHandler(this.btnImportPrices_Click);
            // 
            // lsvStocks
            // 
            this.lsvStocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvStocks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lsvStocks.ContextMenuStrip = this.ctxStocks;
            this.lsvStocks.FullRowSelect = true;
            this.lsvStocks.Location = new System.Drawing.Point(0, 28);
            this.lsvStocks.MultiSelect = false;
            this.lsvStocks.Name = "lsvStocks";
            this.lsvStocks.Size = new System.Drawing.Size(345, 485);
            this.lsvStocks.TabIndex = 1;
            this.lsvStocks.UseCompatibleStateImageBehavior = false;
            this.lsvStocks.View = System.Windows.Forms.View.Details;
            this.lsvStocks.SelectedIndexChanged += new System.EventHandler(this.lsvStocks_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ASX Code";
            this.columnHeader1.Width = 99;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.Width = 383;
            // 
            // ctxStocks
            // 
            this.ctxStocks.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxStocks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDeleteStock,
            this.mnuRenameStock,
            this.mnuChangeASXCode,
            this.mnuDelistStock});
            this.ctxStocks.Name = "ctxStocks";
            this.ctxStocks.Size = new System.Drawing.Size(171, 92);
            // 
            // mnuDeleteStock
            // 
            this.mnuDeleteStock.Name = "mnuDeleteStock";
            this.mnuDeleteStock.Size = new System.Drawing.Size(170, 22);
            this.mnuDeleteStock.Text = "Delete";
            this.mnuDeleteStock.Click += new System.EventHandler(this.mnuDeleteStock_Click);
            // 
            // mnuRenameStock
            // 
            this.mnuRenameStock.Name = "mnuRenameStock";
            this.mnuRenameStock.Size = new System.Drawing.Size(170, 22);
            this.mnuRenameStock.Text = "Rename";
            this.mnuRenameStock.Click += new System.EventHandler(this.mnuRenameStock_Click);
            // 
            // mnuChangeASXCode
            // 
            this.mnuChangeASXCode.Name = "mnuChangeASXCode";
            this.mnuChangeASXCode.Size = new System.Drawing.Size(170, 22);
            this.mnuChangeASXCode.Text = "Change ASX Code";
            this.mnuChangeASXCode.Click += new System.EventHandler(this.mnuChangeASXCode_Click);
            // 
            // mnuDelistStock
            // 
            this.mnuDelistStock.Name = "mnuDelistStock";
            this.mnuDelistStock.Size = new System.Drawing.Size(170, 22);
            this.mnuDelistStock.Text = "Delist";
            this.mnuDelistStock.Click += new System.EventHandler(this.mnuDelistStock_Click);
            // 
            // lsvCorporateActions
            // 
            this.lsvCorporateActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvCorporateActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDate,
            this.colDescription});
            this.lsvCorporateActions.ContextMenuStrip = this.ctxCorporateActions;
            this.lsvCorporateActions.FullRowSelect = true;
            this.lsvCorporateActions.Location = new System.Drawing.Point(351, 28);
            this.lsvCorporateActions.Name = "lsvCorporateActions";
            this.lsvCorporateActions.Size = new System.Drawing.Size(661, 485);
            this.lsvCorporateActions.TabIndex = 2;
            this.lsvCorporateActions.UseCompatibleStateImageBehavior = false;
            this.lsvCorporateActions.View = System.Windows.Forms.View.Details;
            this.lsvCorporateActions.ItemActivate += new System.EventHandler(this.lsvCorporateActions_ItemActivate);
            // 
            // colDate
            // 
            this.colDate.Text = "Date";
            this.colDate.Width = 121;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 476;
            // 
            // ctxCorporateActions
            // 
            this.ctxCorporateActions.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxCorporateActions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditCorporateAction,
            this.mnuDeleteCorporateAction});
            this.ctxCorporateActions.Name = "ctxCorporateActions";
            this.ctxCorporateActions.Size = new System.Drawing.Size(108, 48);
            // 
            // mnuEditCorporateAction
            // 
            this.mnuEditCorporateAction.Name = "mnuEditCorporateAction";
            this.mnuEditCorporateAction.Size = new System.Drawing.Size(107, 22);
            this.mnuEditCorporateAction.Text = "Edit";
            this.mnuEditCorporateAction.Click += new System.EventHandler(this.mnuEditCorporateAction_Click);
            // 
            // mnuDeleteCorporateAction
            // 
            this.mnuDeleteCorporateAction.Name = "mnuDeleteCorporateAction";
            this.mnuDeleteCorporateAction.Size = new System.Drawing.Size(107, 22);
            this.mnuDeleteCorporateAction.Text = "Delete";
            this.mnuDeleteCorporateAction.Click += new System.EventHandler(this.mnuDeleteCorporateAction_Click);
            // 
            // frmStockManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 513);
            this.Controls.Add(this.lsvCorporateActions);
            this.Controls.Add(this.lsvStocks);
            this.Controls.Add(this.toolStrip1);
            this.Name = "frmStockManager";
            this.Text = "Stock Manager";
            this.Shown += new System.EventHandler(this.frmStockManager_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ctxStocks.ResumeLayout(false);
            this.ctxCorporateActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddStock;
        private System.Windows.Forms.ListView lsvStocks;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripDropDownButton btnAddCorporateAction;
        private System.Windows.Forms.ToolStripMenuItem btnAddDividend;
        private System.Windows.Forms.ListView lsvCorporateActions;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ContextMenuStrip ctxStocks;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteStock;
        private System.Windows.Forms.ToolStripMenuItem mnuRenameStock;
        private System.Windows.Forms.ToolStripMenuItem mnuChangeASXCode;
        private System.Windows.Forms.ToolStripMenuItem mnuDelistStock;
        private System.Windows.Forms.ContextMenuStrip ctxCorporateActions;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCorporateAction;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteCorporateAction;
        private System.Windows.Forms.ToolStripMenuItem btnAddCapitalReturn;
        private System.Windows.Forms.ToolStripMenuItem btnAddTransformation;
        private System.Windows.Forms.ToolStripButton btnDownloadDividends;
        private System.Windows.Forms.ToolStripButton btnImportPrices;
        private System.Windows.Forms.ToolStripMenuItem btnAddSplitConsolidation;
        private System.Windows.Forms.ToolStripMenuItem btnAddCompositeAction;
    }
}