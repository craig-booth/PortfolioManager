namespace PortfolioManager.Test
{
    partial class frmTransformation
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
            this.btnCAncel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblASXCode = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpRecordDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpImplementationDate = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtCashComponent = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grdResultingStocks = new System.Windows.Forms.DataGridView();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colResultingStock = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colOriginalUnits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNewunits = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCostBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdResultingStocks)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCAncel
            // 
            this.btnCAncel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCAncel.Location = new System.Drawing.Point(551, 373);
            this.btnCAncel.Name = "btnCAncel";
            this.btnCAncel.Size = new System.Drawing.Size(75, 23);
            this.btnCAncel.TabIndex = 12;
            this.btnCAncel.Text = "Cancel";
            this.btnCAncel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(450, 373);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblASXCode
            // 
            this.lblASXCode.AutoSize = true;
            this.lblASXCode.Location = new System.Drawing.Point(128, 20);
            this.lblASXCode.Name = "lblASXCode";
            this.lblASXCode.Size = new System.Drawing.Size(116, 13);
            this.lblASXCode.TabIndex = 18;
            this.lblASXCode.Text = "XXXX -xxxxxxxxxxxxxxx";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Record Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "ASX Code";
            // 
            // dtpRecordDate
            // 
            this.dtpRecordDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRecordDate.Location = new System.Drawing.Point(131, 47);
            this.dtpRecordDate.Name = "dtpRecordDate";
            this.dtpRecordDate.Size = new System.Drawing.Size(116, 20);
            this.dtpRecordDate.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Implementation Date";
            // 
            // dtpImplementationDate
            // 
            this.dtpImplementationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpImplementationDate.Location = new System.Drawing.Point(131, 73);
            this.dtpImplementationDate.Name = "dtpImplementationDate";
            this.dtpImplementationDate.Size = new System.Drawing.Size(116, 20);
            this.dtpImplementationDate.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(131, 100);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(288, 20);
            this.txtDescription.TabIndex = 22;
            // 
            // txtCashComponent
            // 
            this.txtCashComponent.Location = new System.Drawing.Point(131, 126);
            this.txtCashComponent.Name = "txtCashComponent";
            this.txtCashComponent.Size = new System.Drawing.Size(100, 20);
            this.txtCashComponent.TabIndex = 24;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Cash Component";
            // 
            // grdResultingStocks
            // 
            this.grdResultingStocks.AllowUserToResizeColumns = false;
            this.grdResultingStocks.AllowUserToResizeRows = false;
            this.grdResultingStocks.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.grdResultingStocks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResultingStocks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDelete,
            this.colResultingStock,
            this.colOriginalUnits,
            this.colNewunits,
            this.colCostBase});
            this.grdResultingStocks.Location = new System.Drawing.Point(25, 165);
            this.grdResultingStocks.Name = "grdResultingStocks";
            this.grdResultingStocks.RowHeadersVisible = false;
            this.grdResultingStocks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdResultingStocks.ShowEditingIcon = false;
            this.grdResultingStocks.ShowRowErrors = false;
            this.grdResultingStocks.Size = new System.Drawing.Size(610, 174);
            this.grdResultingStocks.TabIndex = 26;
            this.grdResultingStocks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdResultingStocks_CellContentClick);
            this.grdResultingStocks.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grdResultingStocks_DataError);
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "";
            this.colDelete.Name = "colDelete";
            this.colDelete.Text = "Delete";
            this.colDelete.UseColumnTextForButtonValue = true;
            this.colDelete.Width = 50;
            // 
            // colResultingStock
            // 
            this.colResultingStock.HeaderText = "Resulting Stock ";
            this.colResultingStock.Name = "colResultingStock";
            this.colResultingStock.Width = 250;
            // 
            // colOriginalUnits
            // 
            this.colOriginalUnits.HeaderText = "Original Units";
            this.colOriginalUnits.Name = "colOriginalUnits";
            // 
            // colNewunits
            // 
            this.colNewunits.HeaderText = "New Units";
            this.colNewunits.Name = "colNewunits";
            // 
            // colCostBase
            // 
            this.colCostBase.HeaderText = "Cost Base %";
            this.colCostBase.Name = "colCostBase";
            // 
            // frmTransformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 408);
            this.Controls.Add(this.grdResultingStocks);
            this.Controls.Add(this.txtCashComponent);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dtpImplementationDate);
            this.Controls.Add(this.lblASXCode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpRecordDate);
            this.Controls.Add(this.btnCAncel);
            this.Controls.Add(this.btnOK);
            this.Name = "frmTransformation";
            this.Text = "frmTransformation";
            ((System.ComponentModel.ISupportInitialize)(this.grdResultingStocks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCAncel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblASXCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpRecordDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpImplementationDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtCashComponent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView grdResultingStocks;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private System.Windows.Forms.DataGridViewComboBoxColumn colResultingStock;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOriginalUnits;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNewunits;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCostBase;
    }
}