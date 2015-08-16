namespace PortfolioManager.Test.TransactionControls
{
    partial class DisposalControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtComment = new System.Windows.Forms.TextBox();
            this.txtTransactionCosts = new System.Windows.Forms.TextBox();
            this.txtAveragePrice = new System.Windows.Forms.TextBox();
            this.txtUnits = new System.Windows.Forms.TextBox();
            this.dtpDisposalDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboASXCode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(96, 136);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(350, 94);
            this.txtComment.TabIndex = 33;
            // 
            // txtTransactionCosts
            // 
            this.txtTransactionCosts.Location = new System.Drawing.Point(96, 110);
            this.txtTransactionCosts.Name = "txtTransactionCosts";
            this.txtTransactionCosts.Size = new System.Drawing.Size(100, 20);
            this.txtTransactionCosts.TabIndex = 31;
            // 
            // txtAveragePrice
            // 
            this.txtAveragePrice.Location = new System.Drawing.Point(96, 83);
            this.txtAveragePrice.Name = "txtAveragePrice";
            this.txtAveragePrice.Size = new System.Drawing.Size(100, 20);
            this.txtAveragePrice.TabIndex = 29;
            // 
            // txtUnits
            // 
            this.txtUnits.Location = new System.Drawing.Point(96, 56);
            this.txtUnits.Name = "txtUnits";
            this.txtUnits.Size = new System.Drawing.Size(100, 20);
            this.txtUnits.TabIndex = 27;
            // 
            // dtpDisposalDate
            // 
            this.dtpDisposalDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDisposalDate.Location = new System.Drawing.Point(96, 3);
            this.dtpDisposalDate.Name = "dtpDisposalDate";
            this.dtpDisposalDate.Size = new System.Drawing.Size(100, 20);
            this.dtpDisposalDate.TabIndex = 25;
            this.dtpDisposalDate.ValueChanged += new System.EventHandler(this.dtpDisposalDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "Comment";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Transaction Costs";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "Average Price";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Units";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Disposal Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "ASX Code";
            // 
            // cboASXCode
            // 
            this.cboASXCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboASXCode.FormattingEnabled = true;
            this.cboASXCode.Location = new System.Drawing.Point(96, 29);
            this.cboASXCode.Name = "cboASXCode";
            this.cboASXCode.Size = new System.Drawing.Size(200, 21);
            this.cboASXCode.TabIndex = 24;
            // 
            // DisposalControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.txtTransactionCosts);
            this.Controls.Add(this.txtAveragePrice);
            this.Controls.Add(this.txtUnits);
            this.Controls.Add(this.dtpDisposalDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboASXCode);
            this.Name = "DisposalControl";
            this.Size = new System.Drawing.Size(452, 243);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtTransactionCosts;
        private System.Windows.Forms.TextBox txtAveragePrice;
        private System.Windows.Forms.TextBox txtUnits;
        private System.Windows.Forms.DateTimePicker dtpDisposalDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboASXCode;
    }
}
