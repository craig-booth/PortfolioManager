namespace PortfolioManager.Test
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lsvTransactions = new System.Windows.Forms.ListView();
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lsvCorporateActions = new System.Windows.Forms.ListView();
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader21 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabIncome = new System.Windows.Forms.TabPage();
            this.lsvIncome = new System.Windows.Forms.ListView();
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader29 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabCGT = new System.Windows.Forms.TabPage();
            this.lsvCGT = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabParcels = new System.Windows.Forms.TabPage();
            this.lsvParcels = new System.Windows.Forms.ListView();
            this.columnHeader22 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader23 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader24 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader25 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader26 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader27 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader28 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPortfolio = new System.Windows.Forms.TabPage();
            this.lsvPortfolio = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddStock = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddDividend = new System.Windows.Forms.ToolStripButton();
            this.btnAddCapitalReturn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tabIncome.SuspendLayout();
            this.tabCGT.SuspendLayout();
            this.tabParcels.SuspendLayout();
            this.tabPortfolio.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lsvTransactions
            // 
            this.lsvTransactions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18});
            this.lsvTransactions.Location = new System.Drawing.Point(7, 330);
            this.lsvTransactions.Name = "lsvTransactions";
            this.lsvTransactions.Size = new System.Drawing.Size(488, 261);
            this.lsvTransactions.TabIndex = 6;
            this.lsvTransactions.UseCompatibleStateImageBehavior = false;
            this.lsvTransactions.View = System.Windows.Forms.View.Details;
            this.lsvTransactions.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lsvTransactions_MouseDoubleClick);
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Date";
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "ASX Code";
            this.columnHeader17.Width = 86;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Description";
            this.columnHeader18.Width = 288;
            // 
            // lsvCorporateActions
            // 
            this.lsvCorporateActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader19,
            this.columnHeader20,
            this.columnHeader21});
            this.lsvCorporateActions.Location = new System.Drawing.Point(552, 330);
            this.lsvCorporateActions.Name = "lsvCorporateActions";
            this.lsvCorporateActions.Size = new System.Drawing.Size(547, 261);
            this.lsvCorporateActions.TabIndex = 7;
            this.lsvCorporateActions.UseCompatibleStateImageBehavior = false;
            this.lsvCorporateActions.View = System.Windows.Forms.View.Details;
            this.lsvCorporateActions.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lsvCorporateActions_MouseDoubleClick);
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Date";
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "ASX Code";
            this.columnHeader20.Width = 86;
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "Description";
            this.columnHeader21.Width = 288;
            // 
            // tabIncome
            // 
            this.tabIncome.Controls.Add(this.lsvIncome);
            this.tabIncome.Location = new System.Drawing.Point(4, 22);
            this.tabIncome.Name = "tabIncome";
            this.tabIncome.Size = new System.Drawing.Size(1095, 220);
            this.tabIncome.TabIndex = 2;
            this.tabIncome.Text = "Income";
            this.tabIncome.UseVisualStyleBackColor = true;
            // 
            // lsvIncome
            // 
            this.lsvIncome.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader29});
            this.lsvIncome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvIncome.FullRowSelect = true;
            this.lsvIncome.Location = new System.Drawing.Point(0, 0);
            this.lsvIncome.MultiSelect = false;
            this.lsvIncome.Name = "lsvIncome";
            this.lsvIncome.Size = new System.Drawing.Size(1095, 220);
            this.lsvIncome.TabIndex = 2;
            this.lsvIncome.UseCompatibleStateImageBehavior = false;
            this.lsvIncome.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Date";
            this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader12.Width = 70;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "ASX Code";
            this.columnHeader13.Width = 77;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Amount";
            this.columnHeader14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader14.Width = 133;
            // 
            // columnHeader29
            // 
            this.columnHeader29.Text = "Franking Credits";
            this.columnHeader29.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader29.Width = 142;
            // 
            // tabCGT
            // 
            this.tabCGT.Controls.Add(this.lsvCGT);
            this.tabCGT.Location = new System.Drawing.Point(4, 22);
            this.tabCGT.Name = "tabCGT";
            this.tabCGT.Padding = new System.Windows.Forms.Padding(3);
            this.tabCGT.Size = new System.Drawing.Size(1095, 220);
            this.tabCGT.TabIndex = 1;
            this.tabCGT.Text = "CGT";
            this.tabCGT.UseVisualStyleBackColor = true;
            // 
            // lsvCGT
            // 
            this.lsvCGT.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader8,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader15});
            this.lsvCGT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvCGT.FullRowSelect = true;
            this.lsvCGT.Location = new System.Drawing.Point(3, 3);
            this.lsvCGT.MultiSelect = false;
            this.lsvCGT.Name = "lsvCGT";
            this.lsvCGT.Size = new System.Drawing.Size(1089, 214);
            this.lsvCGT.TabIndex = 1;
            this.lsvCGT.UseCompatibleStateImageBehavior = false;
            this.lsvCGT.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Date";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 70;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "ASX Code";
            this.columnHeader8.Width = 77;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Cost Base";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader10.Width = 125;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Amount Received";
            this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader11.Width = 117;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Capital Gain";
            this.columnHeader15.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader15.Width = 114;
            // 
            // tabParcels
            // 
            this.tabParcels.Controls.Add(this.lsvParcels);
            this.tabParcels.Location = new System.Drawing.Point(4, 22);
            this.tabParcels.Name = "tabParcels";
            this.tabParcels.Size = new System.Drawing.Size(1095, 220);
            this.tabParcels.TabIndex = 3;
            this.tabParcels.Text = "Parcels";
            this.tabParcels.UseVisualStyleBackColor = true;
            // 
            // lsvParcels
            // 
            this.lsvParcels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader22,
            this.columnHeader23,
            this.columnHeader24,
            this.columnHeader25,
            this.columnHeader26,
            this.columnHeader27,
            this.columnHeader28});
            this.lsvParcels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvParcels.FullRowSelect = true;
            this.lsvParcels.Location = new System.Drawing.Point(0, 0);
            this.lsvParcels.MultiSelect = false;
            this.lsvParcels.Name = "lsvParcels";
            this.lsvParcels.Size = new System.Drawing.Size(1095, 220);
            this.lsvParcels.TabIndex = 1;
            this.lsvParcels.UseCompatibleStateImageBehavior = false;
            this.lsvParcels.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "ASX Code";
            this.columnHeader22.Width = 77;
            // 
            // columnHeader23
            // 
            this.columnHeader23.Text = "Units";
            this.columnHeader23.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader23.Width = 70;
            // 
            // columnHeader24
            // 
            this.columnHeader24.Text = "Average Price";
            this.columnHeader24.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader24.Width = 79;
            // 
            // columnHeader25
            // 
            this.columnHeader25.Text = "Cost Base";
            this.columnHeader25.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader25.Width = 77;
            // 
            // columnHeader26
            // 
            this.columnHeader26.Text = "Last Price";
            this.columnHeader26.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader26.Width = 82;
            // 
            // columnHeader27
            // 
            this.columnHeader27.Text = "Market Value";
            this.columnHeader27.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader27.Width = 82;
            // 
            // columnHeader28
            // 
            this.columnHeader28.Text = "Capital Gain";
            this.columnHeader28.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader28.Width = 102;
            // 
            // tabPortfolio
            // 
            this.tabPortfolio.Controls.Add(this.lsvPortfolio);
            this.tabPortfolio.Location = new System.Drawing.Point(4, 22);
            this.tabPortfolio.Name = "tabPortfolio";
            this.tabPortfolio.Padding = new System.Windows.Forms.Padding(3);
            this.tabPortfolio.Size = new System.Drawing.Size(1095, 220);
            this.tabPortfolio.TabIndex = 0;
            this.tabPortfolio.Text = "Portfolio";
            this.tabPortfolio.UseVisualStyleBackColor = true;
            // 
            // lsvPortfolio
            // 
            this.lsvPortfolio.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lsvPortfolio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvPortfolio.FullRowSelect = true;
            this.lsvPortfolio.Location = new System.Drawing.Point(3, 3);
            this.lsvPortfolio.MultiSelect = false;
            this.lsvPortfolio.Name = "lsvPortfolio";
            this.lsvPortfolio.Size = new System.Drawing.Size(1089, 214);
            this.lsvPortfolio.TabIndex = 0;
            this.lsvPortfolio.UseCompatibleStateImageBehavior = false;
            this.lsvPortfolio.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ASX Code";
            this.columnHeader1.Width = 77;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Units";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Average Price";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 79;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Cost Base";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 77;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Last Price";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 84;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Market Value";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader6.Width = 82;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Capital Gain";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 102;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tabControl1.Controls.Add(this.tabPortfolio);
            this.tabControl1.Controls.Add(this.tabParcels);
            this.tabControl1.Controls.Add(this.tabCGT);
            this.tabControl1.Controls.Add(this.tabIncome);
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1103, 246);
            this.tabControl1.TabIndex = 5;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddStock,
            this.toolStripSeparator2,
            this.btnAddDividend,
            this.btnAddCapitalReturn,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1103, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAddDividend
            // 
            this.btnAddDividend.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddDividend.Image = ((System.Drawing.Image)(resources.GetObject("btnAddDividend.Image")));
            this.btnAddDividend.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddDividend.Name = "btnAddDividend";
            this.btnAddDividend.Size = new System.Drawing.Size(83, 22);
            this.btnAddDividend.Text = "Add Dividend";
            this.btnAddDividend.Click += new System.EventHandler(this.btnAddDividend_Click);
            // 
            // btnAddCapitalReturn
            // 
            this.btnAddCapitalReturn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddCapitalReturn.Image = ((System.Drawing.Image)(resources.GetObject("btnAddCapitalReturn.Image")));
            this.btnAddCapitalReturn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddCapitalReturn.Name = "btnAddCapitalReturn";
            this.btnAddCapitalReturn.Size = new System.Drawing.Size(111, 22);
            this.btnAddCapitalReturn.Text = "Add Capital Return";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1103, 603);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lsvCorporateActions);
            this.Controls.Add(this.lsvTransactions);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmMain";
            this.Text = "Portfolio Manager";
            this.tabIncome.ResumeLayout(false);
            this.tabCGT.ResumeLayout(false);
            this.tabParcels.ResumeLayout(false);
            this.tabPortfolio.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lsvTransactions;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ListView lsvCorporateActions;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.ColumnHeader columnHeader21;
        private System.Windows.Forms.TabPage tabIncome;
        private System.Windows.Forms.ListView lsvIncome;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader29;
        private System.Windows.Forms.TabPage tabCGT;
        private System.Windows.Forms.ListView lsvCGT;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.TabPage tabParcels;
        private System.Windows.Forms.ListView lsvParcels;
        private System.Windows.Forms.ColumnHeader columnHeader22;
        private System.Windows.Forms.ColumnHeader columnHeader23;
        private System.Windows.Forms.ColumnHeader columnHeader24;
        private System.Windows.Forms.ColumnHeader columnHeader25;
        private System.Windows.Forms.ColumnHeader columnHeader26;
        private System.Windows.Forms.ColumnHeader columnHeader27;
        private System.Windows.Forms.ColumnHeader columnHeader28;
        private System.Windows.Forms.TabPage tabPortfolio;
        private System.Windows.Forms.ListView lsvPortfolio;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddStock;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnAddDividend;
        private System.Windows.Forms.ToolStripButton btnAddCapitalReturn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

