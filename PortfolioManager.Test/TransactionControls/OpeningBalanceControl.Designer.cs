namespace PortfolioManager.Test.TransactionControls
{
    partial class OpeningBalanceControl
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
            this.txtCostBase = new System.Windows.Forms.TextBox();
            this.txtUnits = new System.Windows.Forms.TextBox();
            this.dtpBalanceDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cboASXCode = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(96, 111);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(350, 94);
            this.txtComment.TabIndex = 32;
            // 
            // txtCostBase
            // 
            this.txtCostBase.Location = new System.Drawing.Point(96, 85);
            this.txtCostBase.Name = "txtCostBase";
            this.txtCostBase.Size = new System.Drawing.Size(100, 20);
            this.txtCostBase.TabIndex = 29;
            // 
            // txtUnits
            // 
            this.txtUnits.Location = new System.Drawing.Point(96, 58);
            this.txtUnits.Name = "txtUnits";
            this.txtUnits.Size = new System.Drawing.Size(100, 20);
            this.txtUnits.TabIndex = 27;
            // 
            // dtpBalanceDate
            // 
            this.dtpBalanceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBalanceDate.Location = new System.Drawing.Point(96, 5);
            this.dtpBalanceDate.Name = "dtpBalanceDate";
            this.dtpBalanceDate.Size = new System.Drawing.Size(100, 20);
            this.dtpBalanceDate.TabIndex = 25;
            this.dtpBalanceDate.ValueChanged += new System.EventHandler(this.dtpBalanceDate_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 33;
            this.label6.Text = "Comment";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Cost Base";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Units";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Balance Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "ASX Code";
            // 
            // cboASXCode
            // 
            this.cboASXCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboASXCode.FormattingEnabled = true;
            this.cboASXCode.Location = new System.Drawing.Point(96, 31);
            this.cboASXCode.Name = "cboASXCode";
            this.cboASXCode.Size = new System.Drawing.Size(200, 21);
            this.cboASXCode.TabIndex = 24;
            // 
            // OpeningBalanceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.txtCostBase);
            this.Controls.Add(this.txtUnits);
            this.Controls.Add(this.dtpBalanceDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboASXCode);
            this.Name = "OpeningBalanceControl";
            this.Size = new System.Drawing.Size(454, 218);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtCostBase;
        private System.Windows.Forms.TextBox txtUnits;
        private System.Windows.Forms.DateTimePicker dtpBalanceDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboASXCode;
    }
}
