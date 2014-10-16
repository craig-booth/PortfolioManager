namespace PortfolioManager.Test
{
    partial class frmDividend
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
            this.cboASXCode = new System.Windows.Forms.ComboBox();
            this.dtpRecordDate = new System.Windows.Forms.DateTimePicker();
            this.dtpPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblPercentFranked = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCAncel = new System.Windows.Forms.Button();
            this.txtDividendAmount = new System.Windows.Forms.TextBox();
            this.txtPercentFranked = new System.Windows.Forms.TextBox();
            this.txtCompanyTaxRate = new System.Windows.Forms.TextBox();
            this.txtDRPPrice = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cboASXCode
            // 
            this.cboASXCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboASXCode.FormattingEnabled = true;
            this.cboASXCode.Location = new System.Drawing.Point(142, 49);
            this.cboASXCode.Name = "cboASXCode";
            this.cboASXCode.Size = new System.Drawing.Size(200, 21);
            this.cboASXCode.TabIndex = 1;
            // 
            // dtpRecordDate
            // 
            this.dtpRecordDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpRecordDate.Location = new System.Drawing.Point(142, 23);
            this.dtpRecordDate.Name = "dtpRecordDate";
            this.dtpRecordDate.Size = new System.Drawing.Size(116, 20);
            this.dtpRecordDate.TabIndex = 0;
            this.dtpRecordDate.ValueChanged += new System.EventHandler(this.dtpRecordDate_ValueChanged);
            // 
            // dtpPaymentDate
            // 
            this.dtpPaymentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPaymentDate.Location = new System.Drawing.Point(142, 77);
            this.dtpPaymentDate.Name = "dtpPaymentDate";
            this.dtpPaymentDate.Size = new System.Drawing.Size(116, 20);
            this.dtpPaymentDate.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "ASX Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Record Date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Payment Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Dividend Amount";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(265, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Company Tax Rate";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "DRP Price";
            // 
            // lblPercentFranked
            // 
            this.lblPercentFranked.AutoSize = true;
            this.lblPercentFranked.Location = new System.Drawing.Point(38, 132);
            this.lblPercentFranked.Name = "lblPercentFranked";
            this.lblPercentFranked.Size = new System.Drawing.Size(86, 13);
            this.lblPercentFranked.TabIndex = 13;
            this.lblPercentFranked.Text = "Percent Franked";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(326, 282);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCAncel
            // 
            this.btnCAncel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCAncel.Location = new System.Drawing.Point(427, 282);
            this.btnCAncel.Name = "btnCAncel";
            this.btnCAncel.Size = new System.Drawing.Size(75, 23);
            this.btnCAncel.TabIndex = 8;
            this.btnCAncel.Text = "Cancel";
            this.btnCAncel.UseVisualStyleBackColor = true;
            // 
            // txtDividendAmount
            // 
            this.txtDividendAmount.Location = new System.Drawing.Point(142, 106);
            this.txtDividendAmount.Name = "txtDividendAmount";
            this.txtDividendAmount.Size = new System.Drawing.Size(100, 20);
            this.txtDividendAmount.TabIndex = 3;
            // 
            // txtPercentFranked
            // 
            this.txtPercentFranked.Location = new System.Drawing.Point(142, 132);
            this.txtPercentFranked.Name = "txtPercentFranked";
            this.txtPercentFranked.Size = new System.Drawing.Size(100, 20);
            this.txtPercentFranked.TabIndex = 4;
            // 
            // txtCompanyTaxRate
            // 
            this.txtCompanyTaxRate.Location = new System.Drawing.Point(382, 129);
            this.txtCompanyTaxRate.Name = "txtCompanyTaxRate";
            this.txtCompanyTaxRate.Size = new System.Drawing.Size(79, 20);
            this.txtCompanyTaxRate.TabIndex = 5;
            // 
            // txtDRPPrice
            // 
            this.txtDRPPrice.Location = new System.Drawing.Point(142, 181);
            this.txtDRPPrice.Name = "txtDRPPrice";
            this.txtDRPPrice.Size = new System.Drawing.Size(100, 20);
            this.txtDRPPrice.TabIndex = 6;
            // 
            // frmDividend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 326);
            this.Controls.Add(this.txtDRPPrice);
            this.Controls.Add(this.txtCompanyTaxRate);
            this.Controls.Add(this.txtPercentFranked);
            this.Controls.Add(this.txtDividendAmount);
            this.Controls.Add(this.btnCAncel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblPercentFranked);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpPaymentDate);
            this.Controls.Add(this.dtpRecordDate);
            this.Controls.Add(this.cboASXCode);
            this.Name = "frmDividend";
            this.Text = "frmDividend";
            this.Load += new System.EventHandler(this.frmDividend_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboASXCode;
        private System.Windows.Forms.DateTimePicker dtpRecordDate;
        private System.Windows.Forms.DateTimePicker dtpPaymentDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblPercentFranked;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCAncel;
        private System.Windows.Forms.TextBox txtDividendAmount;
        private System.Windows.Forms.TextBox txtPercentFranked;
        private System.Windows.Forms.TextBox txtCompanyTaxRate;
        private System.Windows.Forms.TextBox txtDRPPrice;
    }
}