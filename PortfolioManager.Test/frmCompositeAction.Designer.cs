namespace PortfolioManager.Test
{
    partial class frmCompositeAction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCompositeAction));
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblASXCode = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpActionDate = new System.Windows.Forms.DateTimePicker();
            this.btnCAncel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lsvChildActions = new System.Windows.Forms.ListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnDeleteChildAction = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddCapitalReturn = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddDividend = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddTransformation = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddSplitConsolidation = new System.Windows.Forms.ToolStripMenuItem();
            this.colActionType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(169, 88);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(459, 22);
            this.txtDescription.TabIndex = 36;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 92);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 17);
            this.label5.TabIndex = 35;
            this.label5.Text = "Description";
            // 
            // lblASXCode
            // 
            this.lblASXCode.AutoSize = true;
            this.lblASXCode.Location = new System.Drawing.Point(166, 25);
            this.lblASXCode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblASXCode.Name = "lblASXCode";
            this.lblASXCode.Size = new System.Drawing.Size(143, 17);
            this.lblASXCode.TabIndex = 30;
            this.lblASXCode.Text = "XXXX -xxxxxxxxxxxxxxx";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 17);
            this.label2.TabIndex = 29;
            this.label2.Text = "Action Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 28;
            this.label1.Text = "ASX Code";
            // 
            // dtpActionDate
            // 
            this.dtpActionDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpActionDate.Location = new System.Drawing.Point(170, 58);
            this.dtpActionDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtpActionDate.Name = "dtpActionDate";
            this.dtpActionDate.Size = new System.Drawing.Size(153, 22);
            this.dtpActionDate.TabIndex = 27;
            // 
            // btnCAncel
            // 
            this.btnCAncel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCAncel.Location = new System.Drawing.Point(578, 401);
            this.btnCAncel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCAncel.Name = "btnCAncel";
            this.btnCAncel.Size = new System.Drawing.Size(100, 28);
            this.btnCAncel.TabIndex = 26;
            this.btnCAncel.Text = "Cancel";
            this.btnCAncel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(443, 401);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 25;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lsvChildActions
            // 
            this.lsvChildActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colActionType,
            this.colDescription});
            this.lsvChildActions.Location = new System.Drawing.Point(28, 154);
            this.lsvChildActions.Name = "lsvChildActions";
            this.lsvChildActions.Size = new System.Drawing.Size(650, 231);
            this.lsvChildActions.TabIndex = 37;
            this.lsvChildActions.UseCompatibleStateImageBehavior = false;
            this.lsvChildActions.View = System.Windows.Forms.View.Details;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.btnDeleteChildAction});
            this.toolStrip1.Location = new System.Drawing.Point(28, 126);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(120, 27);
            this.toolStrip1.TabIndex = 38;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnDeleteChildAction
            // 
            this.btnDeleteChildAction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDeleteChildAction.Image = ((System.Drawing.Image)(resources.GetObject("btnDeleteChildAction.Image")));
            this.btnDeleteChildAction.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDeleteChildAction.Name = "btnDeleteChildAction";
            this.btnDeleteChildAction.Size = new System.Drawing.Size(57, 24);
            this.btnDeleteChildAction.Text = "Delete";
            this.btnDeleteChildAction.Click += new System.EventHandler(this.btnDeleteChildAction_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddCapitalReturn,
            this.btnAddDividend,
            this.btnAddTransformation,
            this.btnAddSplitConsolidation});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(51, 24);
            this.toolStripDropDownButton1.Text = "Add";
            // 
            // btnAddCapitalReturn
            // 
            this.btnAddCapitalReturn.Name = "btnAddCapitalReturn";
            this.btnAddCapitalReturn.Size = new System.Drawing.Size(212, 26);
            this.btnAddCapitalReturn.Text = "Capital Return";
            this.btnAddCapitalReturn.Click += new System.EventHandler(this.btnAddCapitalReturn_Click);
            // 
            // btnAddDividend
            // 
            this.btnAddDividend.Name = "btnAddDividend";
            this.btnAddDividend.Size = new System.Drawing.Size(212, 26);
            this.btnAddDividend.Text = "Dividend";
            this.btnAddDividend.Click += new System.EventHandler(this.btnAddDividend_Click);
            // 
            // btnAddTransformation
            // 
            this.btnAddTransformation.Name = "btnAddTransformation";
            this.btnAddTransformation.Size = new System.Drawing.Size(212, 26);
            this.btnAddTransformation.Text = "Transformation";
            this.btnAddTransformation.Click += new System.EventHandler(this.btnAddTransformation_Click);
            // 
            // btnAddSplitConsolidation
            // 
            this.btnAddSplitConsolidation.Name = "btnAddSplitConsolidation";
            this.btnAddSplitConsolidation.Size = new System.Drawing.Size(212, 26);
            this.btnAddSplitConsolidation.Text = "Split/Consolidation";
            this.btnAddSplitConsolidation.Click += new System.EventHandler(this.btnAddSplitConsolidation_Click);
            // 
            // colActionType
            // 
            this.colActionType.Text = "Action Type";
            this.colActionType.Width = 125;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 478;
            // 
            // frmCompositeAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 441);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.lsvChildActions);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblASXCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpActionDate);
            this.Controls.Add(this.btnCAncel);
            this.Controls.Add(this.btnOK);
            this.Name = "frmCompositeAction";
            this.Text = "frmCompositeAction";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblASXCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpActionDate;
        private System.Windows.Forms.Button btnCAncel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ListView lsvChildActions;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripButton btnDeleteChildAction;
        private System.Windows.Forms.ToolStripMenuItem btnAddCapitalReturn;
        private System.Windows.Forms.ToolStripMenuItem btnAddDividend;
        private System.Windows.Forms.ToolStripMenuItem btnAddTransformation;
        private System.Windows.Forms.ToolStripMenuItem btnAddSplitConsolidation;
        private System.Windows.Forms.ColumnHeader colActionType;
        private System.Windows.Forms.ColumnHeader colDescription;
    }
}